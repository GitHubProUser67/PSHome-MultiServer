using CustomLogger;
using Horizon.DME.PluginArgs;
using Horizon.MUM.Models;
using Horizon.PluginManager;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.SERVER;
using MultiServerLibrary.Extension;
using Prometheus;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Horizon.DME.Models
{
    public class World : IAsyncDisposable
    {
        private const short MAX_WORLD = 25000;

        private static readonly Counter worldsCreated = Metrics.CreateCounter("dme_worlds_created_total", "Total number of created worlds in DME.");

        private static readonly UniqueIDGenerator _idCounter = new UniqueIDGenerator();

        public int MAX_CLIENTS_PER_WORLD = DmeClass.Settings.MaxClientsPerWorld;

        #region Id Management

        private static readonly ConcurrentDictionary<int, World> _idToWorld = new();
        private readonly ConcurrentDictionary<int, bool> _pIdIsUsed = new();

        private readonly object _ClientIndexlock = new();

        private void RegisterWorld(int MediusWorldId)
        {
            this.MediusWorldId = MediusWorldId;

            short currentMaxWorldSetting = DmeClass.Settings.DmeServerMaxWorld;

            if (currentMaxWorldSetting > MAX_WORLD || currentMaxWorldSetting < 1)
            {
                WorldId = -1;
                LoggerAccessor.LogError($"[DMEWorld] - Invalid parameter for MAX_WORLD (max:{MAX_WORLD}, min:1)!");
                return;
            }
            else if (_idCounter.ActiveCount >= currentMaxWorldSetting)
            {
                WorldId = -1;
                LoggerAccessor.LogError("[DMEWorld] - Max worlds reached!");
                return;
            }

            WorldId = (int)_idCounter.CreateUniqueID();

            _idToWorld.TryAdd(WorldId, this);

            LoggerAccessor.LogInfo($"[DMEWorld] - Registered world with id {WorldId}");
            worldsCreated.Inc();
        }

        private void FreeWorld()
        {
            if (_idToWorld.TryRemove(WorldId, out _))
            {
                _idCounter.ReleaseID((uint)WorldId);
                LoggerAccessor.LogInfo($"[DMEWorld] - Unregistered world with id {WorldId}");
            }
            else
                LoggerAccessor.LogError($"[DMEWorld] - Failed to unregister world with id {WorldId}");
        }

        public static World? GetWorldByMediusWorldId(int MediusWorldId)
        {
            return _idToWorld.Values.FirstOrDefault(world => world.MediusWorldId == MediusWorldId);
        }

        private bool TryRegisterNewClientIndex(out int index)
        {
            lock (_ClientIndexlock)
            {
                for (index = 0; index < _pIdIsUsed.Count; ++index)
                {
                    if (_pIdIsUsed.TryGetValue(index, out bool isUsed) && !isUsed)
                    {
                        _pIdIsUsed[index] = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void UnregisterClientIndex(int index)
        {
            _pIdIsUsed[index] = false;
        }

        #endregion

        #region TokenManagement

        public Dictionary<ushort, List<int>> clientTokens = new();

        #endregion

        public int WorldId { get; protected set; } = -1;

        public int MediusWorldId { get; protected set; } = -1;

        public int ApplicationId { get; protected set; } = 0;

        public int MaxPlayers { get; protected set; } = 0;

        public int SessionMaster { get; protected set; } = 0;

        public bool SelfDestructFlag { get; protected set; } = false;

        public bool ForceDestruct { get; protected set; } = false;

        public bool Destroy => ((WorldTimer.Elapsed.TotalSeconds > DmeClass.GetAppSettingsOrDefault(ApplicationId).GameTimeoutSeconds) || SelfDestructFlag) && _clients.IsEmpty;

        public bool Destroyed { get; protected set; } = false;

        public DateTime? UtcLastJoined { get; protected set; }

        public Stopwatch WorldTimer { get; protected set; } = Stopwatch.StartNew();

        private readonly ConcurrentDictionary<int, DMEObject> _clients = new();

        private readonly ConcurrentQueue<MediusServerJoinGameRequest> _requestQueue = new();
        private readonly ConcurrentQueue<Task> _playerSyncQueue = new();

        private readonly TaskCompletionSource _clientsFlushedTcs = new TaskCompletionSource();

        private readonly Thread? PlayerJoinQueue;

        public MPSClient? Manager { get; } = null;

        public DMEObject[] Clients
        {
            get
            {
                return _clients.Values.ToArray();
            }
        }

        public World(MPSClient manager, int appId, int maxPlayers, int MediusWorldId)
        {
            MaxPlayers = (DmeClass.Settings.MaxClientsOverride != -1) ? DmeClass.Settings.MaxClientsOverride : maxPlayers;

            if (MaxPlayers > MAX_CLIENTS_PER_WORLD)
            {
                LoggerAccessor.LogError($"[DMEWorld] - maxPlayers from {((DmeClass.Settings.MaxClientsOverride != -1) ? "dme config override parameter" : "request")} is higher than MaxClientsPerWorld allowed in DME config, world will not be created!");
                return;
            }

            Manager = manager;
            ApplicationId = appId;

            // populate collection of used player ids
            for (int i = 0; i < MAX_CLIENTS_PER_WORLD; ++i)
                _pIdIsUsed.TryAdd(i, false);

            RegisterWorld(MediusWorldId);

            PlayerJoinQueue = new Thread(RunJoinGameLoop);
            PlayerJoinQueue.Start();
        }

        private async void RunJoinGameLoop()
        {
            object _sync = new();

            while (true)
            {
                lock (_sync)
                {
                    if (Destroyed && _clients.IsEmpty)
                        break;
                }

                if (!Destroyed)
                    await HandleIncomingJoinGame().ConfigureAwait(false);

                await HandleIncomingPlayerSyncTasks().ConfigureAwait(false);

                await Task.Delay(1).ConfigureAwait(false);
            }

            _clientsFlushedTcs.TrySetResult();
        }

        public async ValueTask DisposeAsync()
        {
            FreeWorld();
            Destroyed = true;

            await _clientsFlushedTcs.Task.ConfigureAwait(false);

            LoggerAccessor.LogInfo($"[DMEWorld] - world:{WorldId} destroyed.");

            GC.SuppressFinalize(this);
        }

        public async Task Stop()
        {
            // Stop all clients
            await Task.WhenAll(_clients.Select(x => x.Value.Stop())).ConfigureAwait(false);

            _requestQueue.Clear();

            await DisposeAsync().ConfigureAwait(false);
        }

        public Task EnqueueJoinGame(MediusServerJoinGameRequest request)
        {
            if (!Destroyed)
                _requestQueue.Enqueue(request);
            return Task.CompletedTask;
        }

        public Task EnqueuePlayerSyncTask(Task task)
        {
            if (!Destroyed)
                _playerSyncQueue.Enqueue(task);
            return Task.CompletedTask;
        }

        public async Task HandleIncomingJoinGame()
        {
            try
            {
                while (_requestQueue.TryDequeue(out MediusServerJoinGameRequest? request))
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    Manager?.Enqueue(await OnJoinGameRequest(request).ConfigureAwait(false));
                }
            }
            catch
            {
            }
        }

        public async Task HandleIncomingPlayerSyncTasks()
        {
            try
            {
                while (_playerSyncQueue.TryDequeue(out Task? task))
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    await task.ConfigureAwait(false);
                }
            }
            catch
            {
            }
        }

        public Task HandleIncomingMessages()
        {
            List<Task> tasks = new();

            // Process clients
            for (int i = 0; i < MAX_CLIENTS_PER_WORLD; ++i)
            {
                if (_clients.TryGetValue(i, out DMEObject? client))
                    tasks.Add(client.HandleIncomingMessages());
            }

            return Task.WhenAll(tasks);
        }

        public async Task HandleOutgoingMessages()
        {
            // Process clients
            for (int i = 0; i < MAX_CLIENTS_PER_WORLD; ++i)
            {
                if (_clients.TryGetValue(i, out DMEObject? client))
                {
                    if (client.Destroy || ForceDestruct || Destroyed)
                    {
                        if (_clients.TryRemove(i, out DMEObject? client2))
                        {
                            _ = EnqueuePlayerSyncTask(OnPlayerLeft(client2)
                                        .ContinueWith(x =>
                                        {
                                            Manager?.RemoveClient(client2);
                                            return client2.Stop();
                                        }));
                        }
                    }
                    else if (client.Timedout)
                        client.ForceDisconnect();
                    else if (client.IsAggTime)
                        client.HandleOutgoingMessages();
                }
            }

            // Remove
            if (Destroy)
            {
                if (!Destroyed)
                {
                    LoggerAccessor.LogWarn($"[DMEWorld] - destroying world:{WorldId}.");
                    await Stop();
                }

                Manager?.RemoveWorld(this);
            }
        }

        public void MoveGameWorldId(int NewGameMediusWorldID)
        {
            MediusWorldId = NewGameMediusWorldID;
        }

        #region Send

        public void BroadcastTcpScertMessage(BaseScertMessage msg)
        {
            foreach (var client in _clients)
            {
                if (!client.Value.IsAuthenticated || !client.Value.IsConnected || !client.Value.HasRecvFlag(RT_RECV_FLAG.RECV_BROADCAST))
                    continue;

                client.Value.EnqueueTcp(msg);
            }
        }

        public void BroadcastTcp(DMEObject source, byte[] Payload, Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null)
        {
            foreach (var target in _clients.Values)
            {
                if (target == source || !target.IsAuthenticated || !target.IsConnected || !target.HasRecvFlag(RT_RECV_FLAG.RECV_BROADCAST))
                    continue;

                var message = new RT_MSG_CLIENT_APP_SINGLE()
                {
                    TargetOrSource = (short)source.DmeId,
                    Payload = Payload
                };

                modifyMessagePerClient?.Invoke(message, target);

                target.EnqueueTcp(message);
            }
        }

        public void BroadcastUdp(DMEObject source, byte[] Payload, Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null)
        {
            foreach (var target in _clients.Values)
            {
                if (target == source || !target.IsAuthenticated || !target.IsConnected || !target.HasRecvFlag(RT_RECV_FLAG.RECV_BROADCAST))
                    continue;

                var message = new RT_MSG_CLIENT_APP_SINGLE()
                {
                    TargetOrSource = (short)source.DmeId,
                    Payload = Payload
                };

                modifyMessagePerClient?.Invoke(message, target);

                target.EnqueueUdp(message);
            }
        }

        public void SendTcpAppList(DMEObject source, List<int> targetDmeIds, byte[] Payload, Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null)
        {
            foreach (int targetId in targetDmeIds)
            {
                if (_clients.TryGetValue(targetId, out DMEObject? target))
                {
                    if (target == null || !target.IsAuthenticated || !target.IsConnected || !target.HasRecvFlag(RT_RECV_FLAG.RECV_LIST))
                        continue;

                    var message = new RT_MSG_CLIENT_APP_SINGLE()
                    {
                        TargetOrSource = (short)source.DmeId,
                        Payload = Payload
                    };

                    modifyMessagePerClient?.Invoke(message, target);

                    target.EnqueueTcp(message);
                }
            }
        }

        public void SendUdpAppList(DMEObject source, List<int> targetDmeIds, byte[] Payload, Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null)
        {
            foreach (int targetId in targetDmeIds)
            {
                if (_clients.TryGetValue(targetId, out DMEObject? target))
                {
                    if (target == null || !target.IsAuthenticated || !target.IsConnected || !target.HasRecvFlag(RT_RECV_FLAG.RECV_LIST))
                        continue;

                    var message = new RT_MSG_CLIENT_APP_SINGLE()
                    {
                        TargetOrSource = (short)source.DmeId,
                        Payload = Payload
                    };

                    modifyMessagePerClient?.Invoke(message, target);

                    target.EnqueueUdp(message);
                }
            }
        }

        public void SendTcpAppSingle(DMEObject source, short targetDmeId, byte[] Payload, Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null)
        {
            DMEObject? target = _clients.FirstOrDefault(x => x.Value.DmeId == targetDmeId).Value;

            if (target != null && target.IsAuthenticated && target.IsConnected && target.HasRecvFlag(RT_RECV_FLAG.RECV_SINGLE))
            {
                var message = new RT_MSG_CLIENT_APP_SINGLE()
                {
                    TargetOrSource = (short)source.DmeId,
                    Payload = Payload
                };

                modifyMessagePerClient?.Invoke(message, target);

                target.EnqueueTcp(message);
            }
        }

        public void SendUdpAppSingle(DMEObject source, short targetDmeId, byte[] Payload)
        {
            DMEObject? target = _clients.FirstOrDefault(x => x.Value.DmeId == targetDmeId).Value;

            if (target != null && target.IsAuthenticated && target.IsConnected && target.HasRecvFlag(RT_RECV_FLAG.RECV_SINGLE))
            {
                target.EnqueueUdp(new RT_MSG_CLIENT_APP_SINGLE()
                {
                    TargetOrSource = (short)source.DmeId,
                    Payload = Payload
                });
            }
        }

        #endregion

        #region Message Handlers

        public void OnEndGameRequest(MediusServerEndGameRequest request)
        {
            SelfDestructFlag = true;
            ForceDestruct = request.BrutalFlag;
        }

        public Task OnPlayerJoined(DMEObject player)
        {
            if (player.RemoteUdpEndpoint == null)
            {
                LoggerAccessor.LogError($"[World] - OnPlayerJoined - player {player.IP} on ApplicationId {player.ApplicationId} has no UdpEndpoint!");
                return Task.CompletedTask;
            }

            player.HasJoined = true;
                UtcLastJoined = DateTimeUtils.GetHighPrecisionUtcTime();

                // Plugin
                DmeClass.Plugins.OnEvent(PluginEvent.DME_PLAYER_ON_JOINED, new OnPlayerArgs()
                {
                    Player = player,
                    Game = this
                }).Wait();

                // Tell other clients
                foreach (var client in _clients)
                {
                    if (!client.Value.HasJoined || client.Value == player || !client.Value.HasRecvFlag(RT_RECV_FLAG.RECV_NOTIFICATION))
                        continue;

                    client.Value.EnqueueTcp(new RT_MSG_SERVER_CONNECT_NOTIFY()
                    {
                        PlayerIndex = (short)player.DmeId,
                        ScertId = (short)player.ScertId,
                        IP = player.RemoteUdpEndpoint.Address
                    });
                }

                _ = Task.Run(() => {
                    List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> tokenList = new();

                    lock (clientTokens)
                    {
                        foreach (var token in clientTokens.Keys)
                        {
                            if (clientTokens.TryGetValue(token, out List<int>? value) && value != null && value.Count > 0)
                                tokenList.Add((RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNED, token, (ushort)value[0]));
                        }
                    }

                    if (tokenList.Count > 0) // We need to actualize client with every owned tokens.
                        player.EnqueueTcp(new RT_MSG_SERVER_TOKEN_MESSAGE()
                        {
                            TokenList = tokenList
                        });
                });

            // Tell server
            Manager?.Enqueue(new MediusServerConnectNotification()
            {
                MediusWorldUID = MediusWorldId,
                PlayerSessionKey = player.SessionKey ?? string.Empty,
                ConnectEventType = MGCL_EVENT_TYPE.MGCL_EVENT_CLIENT_CONNECT
            });

            return Task.CompletedTask;
        }

        private Task OnPlayerLeft(DMEObject player)
        {
            if (player.RemoteUdpEndpoint == null)
            {
                LoggerAccessor.LogError($"[World] - OnPlayerLeft - player {player.IP} on ApplicationId {player.ApplicationId} has no UdpEndpoint!");
                return Task.CompletedTask;
            }

            player.HasJoined = false;

                // Plugin
                DmeClass.Plugins.OnEvent(PluginEvent.DME_PLAYER_ON_LEFT, new OnPlayerArgs()
                {
                    Player = player,
                    Game = this
                }).Wait();

                if (player.MediusVersion >= 109)
                {
                    // Migrate session master
                    if (player.DmeId == SessionMaster && _clients.Any())
                    {
                        DMEObject? preferredHost = _clients.ToArray()
                            .Select(client => client.Value)
                            .Where(client => client != player)
                            .OrderBy(client => client.DmeId)
                            .FirstOrDefault();

                        if (preferredHost != null)
                        {
                            SessionMaster = preferredHost.DmeId;
                            LoggerAccessor.LogWarn($"[DMEWorld] - Session master migrated to client {SessionMaster}");
                        }
                    }
                }

                // Tell other clients
                foreach (var client in _clients)
                {
                    if (!client.Value.HasJoined || client.Value == player || !client.Value.HasRecvFlag(RT_RECV_FLAG.RECV_NOTIFICATION))
                        continue;

                    client.Value.EnqueueTcp(new RT_MSG_SERVER_DISCONNECT_NOTIFY()
                    {
                        PlayerIndex = (short)player.DmeId,
                        ScertId = (short)player.ScertId,
                        IP = player.RemoteUdpEndpoint.Address
                    });
                }

            // Tell server
            Manager?.Enqueue(new MediusServerConnectNotification()
            {
                MediusWorldUID = MediusWorldId,
                PlayerSessionKey = player.SessionKey ?? string.Empty,
                ConnectEventType = MGCL_EVENT_TYPE.MGCL_EVENT_CLIENT_DISCONNECT
            });

            return Task.CompletedTask;
        }
#pragma warning disable
        public async Task<MediusServerJoinGameResponse> OnJoinGameRequest(MediusServerJoinGameRequest request)
        {
            DMEObject newClient;

            // find existing client and reuse
            KeyValuePair<int, DMEObject> existingClient = _clients.FirstOrDefault(x => x.Value.SessionKey == request.ConnectInfo.SessionKey);
            if (existingClient.Value != null)
            {
                // found existing
                /*if (DmeClass.GetAppSettingsOrDefault(ApplicationId).EnableDmeEncryption)
                    return new MediusServerJoinGameResponse()
                    {
                        MessageID = request.MessageID,
                        DmeClientIndex = existingClient.Value.DmeId,
                        AccessKey = request.ConnectInfo.AccessKey,
                        Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                        pubKey = request.ConnectInfo.ServerKey
                    };
                else*/
                    return new MediusServerJoinGameResponse()
                    {
                        MessageID = request.MessageID,
                        DmeClientIndex = existingClient.Value.DmeId,
                        AccessKey = request.ConnectInfo.AccessKey,
                        Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS
                    };
            }

            // If world is full then fail
            if (_clients.Count >= MAX_CLIENTS_PER_WORLD)
            {
                LoggerAccessor.LogWarn($"[DMEWorld] - Player attempted to join world {this} but world is full!");
                return new MediusServerJoinGameResponse()
                {
                    MessageID = request.MessageID,
                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL
                };
            }

            ClientObject? mumClient = MediusClass.Manager.GetClientBySessionKey(request.ConnectInfo.SessionKey, this.ApplicationId);

            if (mumClient == null)
            {
                LoggerAccessor.LogWarn($"[DMEWorld] - Player attempted to join world {this} but it has no MUM Clients available!");
                return new MediusServerJoinGameResponse()
                {
                    MessageID = request.MessageID,
                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL
                };
            }

            if (TryRegisterNewClientIndex(out int newClientIndex))
            {
                if (!_clients.TryAdd(newClientIndex, newClient = new DMEObject(request.ConnectInfo.SessionKey, this, newClientIndex, mumClient)))
                {
                    UnregisterClientIndex(newClientIndex);
                    return new MediusServerJoinGameResponse()
                    {
                        MessageID = request.MessageID,
                        Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL
                    };
                }
                else
                {
                    newClient.ApplicationId = this.ApplicationId;
                    newClient.OnDestroyed = (client) => {
                        UnregisterClientIndex(client.DmeId);
                        LoggerAccessor.LogWarn($"[DMEWorld] - Player:{client} left world {this}, {client.DmeId} Freed.");
                    };
                }
            }
            else
            {
                LoggerAccessor.LogWarn($"Player attempted to join world {this} but unable to add player!");
                return new MediusServerJoinGameResponse()
                {
                    MessageID = request.MessageID,
                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL
                };
            }

            // Add client to manager
            Manager.AddClient(newClient);

            /*if (DmeClass.GetAppSettingsOrDefault(ApplicationId).EnableDmeEncryption)
                return new MediusServerJoinGameResponse()
                {
                    MessageID = request.MessageID,
                    DmeClientIndex = newClient.DmeId,
                    AccessKey = request.ConnectInfo.AccessKey,
                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                    pubKey = request.ConnectInfo.ServerKey
                };
            else*/
                return new MediusServerJoinGameResponse()
                {
                    MessageID = request.MessageID,
                    DmeClientIndex = newClient.DmeId,
                    AccessKey = request.ConnectInfo.AccessKey,
                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS
                };
        }
#pragma warning restore
        #endregion
    }
}
