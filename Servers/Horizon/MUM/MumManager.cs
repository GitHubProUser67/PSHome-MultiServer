using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Database.Models;
using System.Collections.Concurrent;
using System.Net;
using IChannel = DotNetty.Transport.Channels.IChannel;
using Horizon.HTTPSERVICE;
using Horizon.SERVER;
using Horizon.SERVER.Medius;
using Horizon.MUM.Models;
using NetworkLibrary.Extension;
using Prometheus;

namespace Horizon.MUM
{
    public class MumManager
    {
        public class QuickLookup
        {
            public Dictionary<int, ClientObject> AccountIdToClient = new();
            public Dictionary<string, ClientObject> AccountNameToClient = new();
            public Dictionary<string, ClientObject> AccessTokenToClient = new();
            public Dictionary<string, ClientObject> SessionKeyToClient = new();

            public Dictionary<int, AccountDTO> BuddyInvitationsToClient = new();

            public Dictionary<int, List<Channel>> AppIdToChannel = new();
            public Dictionary<int, Game> GameIdToGame = new();
            public Dictionary<int, Party> PartyIdToGame = new();

            public Dictionary<int, Clan> ClanIdToClan = new();
            public Dictionary<string, Clan> ClanNameToClan = new();
        }

        public static Counter playersJoined = Metrics.CreateCounter("medius_players_joined_total", "Total number of players having joined Medius.");
        public static Counter channelsCreated = Metrics.CreateCounter("medius_channels_created_total", "Total number of created channels in Medius.");
        private static Counter gamesCreated = Metrics.CreateCounter("medius_games_created_total", "Total number of created games in Medius.");

        private const int gameJoinDelay = 2500;

        private Dictionary<string, int[]> _appIdGroups = new();

        private readonly ConcurrentDictionary<(int, int), SemaphoreSlim> _gameJoinSemaphores = new ConcurrentDictionary<(int, int), SemaphoreSlim>();
        private readonly ConcurrentDictionary<(int, int), SemaphoreSlim> _gameJoin0Semaphores = new ConcurrentDictionary<(int, int), SemaphoreSlim>();

        private readonly ConcurrentDictionary<(int, int), ConcurrentQueue<Func<Task>>> _gameJoinRequestQueue = new ConcurrentDictionary<(int, int), ConcurrentQueue<Func<Task>>>();
        private readonly ConcurrentDictionary<(int, int), ConcurrentQueue<Func<Task>>> _gameJoin0RequestQueue = new ConcurrentDictionary<(int, int), ConcurrentQueue<Func<Task>>>();

        private readonly ConcurrentDictionary<int, QuickLookup> _lookupsByAppId = new();

        private readonly List<MediusFile> _mediusFiles = new();
        private readonly List<MediusFileMetaData> _mediusFilesToUpdateMetaData = new();

        private readonly ConcurrentQueue<ClientObject> _addQueue = new();

        #region Clients
        public List<ClientObject> GetClients(int appId)
        {
            return appId == 0
                ? _lookupsByAppId.SelectMany(x => x.Value.SessionKeyToClient.Select(x => x.Value)).ToList()
                : _lookupsByAppId
                    .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                    .SelectMany(x => x.Value.SessionKeyToClient.Select(x => x.Value))
                    .ToList();
        }

        public ClientObject? GetClientByAccountId(int accountId, int appId)
        {
            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    if (quickLookup.AccountIdToClient.TryGetValue(accountId, out ClientObject? result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientByAccountName(string accountName, int appId)
        {
            accountName = accountName.ToLower();

            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    if (quickLookup.AccountNameToClient.TryGetValue(accountName, out ClientObject? result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientByAccessToken(string? accessToken, int appId)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    if (quickLookup.AccessTokenToClient.TryGetValue(accessToken, out ClientObject? result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientBySessionKey(string? sessionKey, int appId)
        {
            if (string.IsNullOrEmpty(sessionKey))
                return null;

            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    if (quickLookup.SessionKeyToClient.TryGetValue(sessionKey, out ClientObject? result))
                        return result;
                }
            }

            return null;
        }

        public List<ClientObject>? GetClientsByIp(string? Ip, int appId)
        {
            return _lookupsByAppId
                   .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                   .SelectMany(x => x.Value.SessionKeyToClient.Select(x => x.Value))
                   .Where(x => x.ApplicationId == appId && x.IP.ToString().Equals(Ip))
                   .ToList();
        }

        public void AddClient(ClientObject client)
        {
            _addQueue.Enqueue(client);
        }

        public void AddOrUpdateLoggedInClient(ClientObject newClient)
        {
            if (!newClient.IsLoggedIn)
            {
                LoggerAccessor.LogError("[MumManager] - Trying to add a LoggedIn client but client is not logged in!");
                return;
            }

            foreach (int appIdInGroup in GetAppIdsInGroup(newClient.ApplicationId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(newClient.AccountName))
                        {
                            string accountNameLower = newClient.AccountName.ToLower();

                            if (quickLookup.AccountIdToClient.ContainsKey(newClient.AccountId))
                                quickLookup.AccountIdToClient[newClient.AccountId] = newClient;
                            else
                                quickLookup.AccountIdToClient.Add(newClient.AccountId, newClient);

                            if (quickLookup.AccountNameToClient.ContainsKey(accountNameLower))
                                quickLookup.AccountNameToClient[accountNameLower] = newClient;
                            else
                                quickLookup.AccountNameToClient.Add(accountNameLower, newClient);
                        }
                    }
                    catch (Exception e)
                    {
                        // clean up
                        if (!string.IsNullOrEmpty(newClient.AccountName))
                        {
                            quickLookup.AccountIdToClient.Remove(newClient.AccountId);
                            quickLookup.AccountNameToClient.Remove(newClient.AccountName.ToLower());
                        }

                        LoggerAccessor.LogError(e);
                    }
                }
            }
        }

        #endregion

        #region Games

        public uint GetGameCount(int appId)
        {
            var appIdsInGroup = GetAppIdsInGroup(appId);
            uint count = 0;

            foreach (var appIdInGroup in appIdsInGroup)
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    lock (quickLookup.GameIdToGame)
                    {
                        count += (uint)quickLookup.GameIdToGame.Count();
                    }
                }
            }

            return count;
        }

        public Game? GetGameByMediusWorldId(string dmeSessionKey, int MediusWorldId)
        {
            if (string.IsNullOrEmpty(dmeSessionKey))
                return null;

            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.GameIdToGame)
                {
                    Game? game = lookupByAppId.Value.GameIdToGame.FirstOrDefault(x => x.Value?.DMEServer?.SessionKey == dmeSessionKey && x.Value?.MediusWorldId == MediusWorldId).Value;
                    if (game != null)
                        return game;
                }
            }

            return null;
        }

        public Party? GetPartyByMediusWorldId(string dmeSessionKey, int MediusWorldId)
        {
            if (string.IsNullOrEmpty(dmeSessionKey))
                return null;

            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.PartyIdToGame)
                {
                    Party? party = lookupByAppId.Value.PartyIdToGame.FirstOrDefault(x => x.Value?.DMEServer?.SessionKey == dmeSessionKey && x.Value?.MediusWorldId == MediusWorldId).Value;
                    if (party != null)
                        return party;
                }
            }

            return null;
        }

        public Party? GetPartyAll(string name, int appId)
        {
            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.PartyIdToGame)
                {
                    Party? party = lookupByAppId.Value.PartyIdToGame.FirstOrDefault(x => x.Value?.ApplicationId == appId && x.Value.PartyName == name).Value;
                    if (party != null)
                        return party;
                }
            }
            return null;
        }

        public Channel? GetWorldByName(string worldName)
        {
            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.AppIdToChannel)
                {
                    Channel? channel = lookupByAppId.Value.AppIdToChannel.SelectMany(kv => kv.Value) // Flatten all channels
                                      .FirstOrDefault(c => c.Name == worldName); // Find the first channel with matching name

                    if (channel != null)
                        return channel;
                }
            }

            return null;
        }

        public Game? GetGameByGameId(int gameId)
        {
            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.GameIdToGame)
                {
                    if (lookupByAppId.Value.GameIdToGame.TryGetValue(gameId, out Game? game))
                        return game;
                }
            }

            return null;
        }

        public IEnumerable<Game> GetAllGamesByAppId(int applicationId)
        {
            if (_lookupsByAppId.TryGetValue(applicationId, out QuickLookup? quickLookup))
            {
                lock (quickLookup.GameIdToGame)
                {
                    foreach (var pair in quickLookup.GameIdToGame)
                    {
                        if (pair.Value.GameChannel != null && pair.Value.GameChannel.ApplicationId == applicationId)
                            yield return pair.Value;
                    }
                }
            }
        }
		
        public List<Game> GetGamesByGameIdViaChannelFilter(int gameId)
        {
            List<Channel> channels = new();
            List<Game> Games = new();

            foreach (int[] appIds in _appIdGroups.Values)
            {
                foreach (int appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out QuickLookup? quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(quickLookup.AppIdToChannel
                                .Where(pair => pair.Key == appId)
                                .SelectMany(pair => pair.Value)
                                .ToList());
                        }
                    }
                }
            }

            foreach (Channel channel in channels)
            {
                foreach (Game game in channel._games)
                {
                    if (game.MediusWorldId == gameId)
                        Games.Add(game);
                }
            }

            return Games;
        }

        public List<Game> GetGamesByGameNameViaChannelFilter(string gameName)
        {
            List<Channel> channels = new();
            List<Game> Games = new();

            foreach (int[] appIds in _appIdGroups.Values)
            {
                foreach (int appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out QuickLookup? quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(quickLookup.AppIdToChannel
                                .Where(pair => pair.Key == appId)
                                .SelectMany(pair => pair.Value)
                                .ToList());
                        }
                    }
                }
            }

            foreach (Channel channel in channels)
            {
                foreach (Game game in channel._games)
                {
                    if (game.GameName == gameName)
                        Games.Add(game);
                }
            }

            return Games;
        }

        public Party? GetPartyByPartyId(int partyId)
        {
            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.PartyIdToGame)
                {
                    if (lookupByAppId.Value.PartyIdToGame.TryGetValue(partyId, out Party? party))
                        return party;
                }
            }

            return null;
        }

        public async Task AddGame(Game game)
        {
            if (!_lookupsByAppId.TryGetValue(game.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(game.ApplicationId, quickLookup = new QuickLookup());

            quickLookup.GameIdToGame.Add(game.MediusWorldId, game);
            await HorizonServerConfiguration.Database.CreateGame(game.ToGameDTO());

            gamesCreated.Inc();
        }

        public int GetGameCountAppId(int appId)
        {
            if (!_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                _lookupsByAppId.TryAdd(appId, quickLookup = new QuickLookup());

            return quickLookup.GameIdToGame.Count;
        }

        public IEnumerable<Game> GetGameList(int appId, int pageIndex, int pageSize, IEnumerable<GameListFilter> filters)
        {
#if DEBUG
            return _lookupsByAppId.Where(x =>
            {
                bool condition = GetAppIdsInGroup(appId).Contains(x.Key);
                if (!condition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameList - Condition failed: AppId {x.Key} is not in the group.");
                return condition;
            })
            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
            .Where(x =>
            {
                bool worldStatusCondition = x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging;
                if (!worldStatusCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameList - Condition failed: WorldStatus is {x.WorldStatus}.");

                bool playerCountCondition = x.PlayerCount >= x.MinPlayers;
                if (!playerCountCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameList - Condition failed: PlayerCount is {x.PlayerCount}, MinPlayers is {x.MinPlayers}.");

                bool filtersCondition = !filters.Any() || filters.All(y => y.IsMatch(x));
                if (!filtersCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameList - Condition failed: Filters do not match.");

                return worldStatusCondition && playerCountCondition && filtersCondition;
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
#else
            return _lookupsByAppId.Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                            .Where(x => (x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging) &&
                                        x.PlayerCount >= x.MinPlayers && (!filters.Any() || filters.All(y => y.IsMatch(x))))
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize);
#endif
        }

        public IEnumerable<Game> GetGameListOnAnyMatchingFilter(int appId, int pageIndex, int pageSize, IEnumerable<GameListFilter> filters)
        {
#if DEBUG
            return _lookupsByAppId.Where(x =>
            {
                bool condition = GetAppIdsInGroup(appId).Contains(x.Key);
                if (!condition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: AppId {x.Key} is not in the group.");
                return condition;
            })
            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
            .Where(x =>
            {
                bool worldStatusCondition = x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging;
                if (!worldStatusCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: WorldStatus is {x.WorldStatus}.");

                bool playerCountCondition = x.PlayerCount >= x.MinPlayers;
                if (!playerCountCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: PlayerCount is {x.PlayerCount}, MinPlayers is {x.MinPlayers}.");

                bool filtersCondition = !filters.Any() || filters.Any(y => y.IsMatch(x));
                if (!filtersCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: No filters matched.");

                return worldStatusCondition && playerCountCondition && filtersCondition;
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
#else
            return _lookupsByAppId.Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                            .Where(x => (x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging) &&
                                        x.PlayerCount >= x.MinPlayers && (!filters.Any() || filters.Any(y => y.IsMatch(x))))
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize);
#endif
        }

        public IEnumerable<Game> GetGameListAppId(int appId, int pageIndex, int pageSize)
        {
#if DEBUG
            return _lookupsByAppId.Where(x =>
            {
                bool condition = GetAppIdsInGroup(appId).Contains(x.Key);
                if (!condition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListAppId - Condition failed: AppId {x.Key} is not in the group.");
                return condition;
            })
            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
            .Where(x =>
            {
                bool worldStatusCondition = x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging;
                if (!worldStatusCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListAppId - Condition failed: WorldStatus is {x.WorldStatus}.");

                bool playerCountCondition = x.PlayerCount >= x.MinPlayers;
                if (!playerCountCondition)
                    LoggerAccessor.LogWarn($"[MumManager] - DEBUG - GetGameListAppId - Condition failed: PlayerCount is {x.PlayerCount}, MinPlayers is {x.MinPlayers}.");

                return worldStatusCondition && playerCountCondition;
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
#else
            return _lookupsByAppId.Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                            .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                            .Where(x => (x.WorldStatus == MediusWorldStatus.WorldActive || x.WorldStatus == MediusWorldStatus.WorldStaging) && x.PlayerCount >= x.MinPlayers)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize);
#endif
        }

        #region CreateGame
        public async Task CreateGame(ClientObject client, IMediusRequest request)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            string? gameName = null;
            Game? game = null;
            if (request is MediusCreateGameRequest r)
            {
                gameName = r.GameName;
            }
            else if (request is MediusCreateGameRequest0 r0)
                gameName = r0.GameName;

            IEnumerable<Game> existingGames = _lookupsByAppId.Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId)).SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (existingGames.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.GameName == gameName && x.Host != null && x.Host.IsConnected))
            {
                client.Queue(new RT_MSG_SERVER_APP()
                {
                    Message = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return;
            }

            // Try to get next free dme server
            // If none exist, return error to clist
            ClientObject? dme = MediusClass.ProxyServer.GetFreeDme(client.ApplicationId, client.LocationId);

            if (dme == null)
            {
                client.Queue(new MediusCreateGameResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusTransactionTimedOut
                });
                return;
            }
            else
            {
                // Create and add game.
                try
                {
                    game = new Game(client, request, dme);

                    await AddGame(game);

                    // Send create game request to dme server
                    dme.Queue(new MediusServerCreateGameWithAttributesRequest()
                    {
                        MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                        WorldID = game.MediusWorldId,
                        Attributes = game.Attributes,
                        ApplicationID = client.ApplicationId,
                        MaxClients = game.MaxPlayers
                    });

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(e);
                }
            }

            // Failure adding game for some reason
            client.Queue(new MediusCreateGameResponse()
            {
                MessageID = request.MessageID,
                MediusWorldID = -1,
                StatusCode = MediusCallbackStatus.MediusFail
            });
        }
        #endregion

        #region CreateGame1
        public async Task CreateGame1(ClientObject client, IMediusRequest request)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            MGCL_GAME_HOST_TYPE? HostType = null;
            string? gameName = null;
            if (request is MediusCreateGameRequest1 r)
            {
                HostType = r.GameHostType;
                gameName = r.GameName;
            }

            IEnumerable<Game> existingGames = _lookupsByAppId.Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId)).SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (existingGames.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.GameName == gameName && x.Host != null && x.Host.IsConnected))
            {
                client.Queue(new RT_MSG_SERVER_APP()
                {
                    Message = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return;
            }

            // Try to get next free dme server
            // If none exist, return error to clist
            ClientObject? dme = MediusClass.ProxyServer.GetFreeDme(client.ApplicationId, client.LocationId);

            if (dme == null)
            {
                client.Queue(new MediusCreateGameResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusTransactionTimedOut
                });
                return;
            }
            else
            {
                // Create and add game.
                try
                {
                    Game game = new(client, request, dme);

                    await AddGame(game);

                    // Send create game request to dme server
                    dme.Queue(new MediusServerCreateGameWithAttributesRequest()
                    {
                        MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                        WorldID = game.MediusWorldId,
                        Attributes = game.Attributes,
                        ApplicationID = client.ApplicationId,
                        MaxClients = game.MaxPlayers
                    });

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(e);
                }
            }

            // Failure adding game for some reason
            client.Queue(new MediusCreateGameResponse()
            {
                MessageID = request.MessageID,
                MediusWorldID = -1,
                StatusCode = MediusCallbackStatus.MediusFail
            });
        }
        #endregion

        #region MatchCreateGame
        public async Task MatchCreateGame(ClientObject client, MediusMatchCreateGameRequest matchCreateGameRequest, IChannel channel)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            string? gameName = null;
            if (matchCreateGameRequest is MediusMatchCreateGameRequest r)
                gameName = r.GameName;

            IEnumerable<Game> existingGames = _lookupsByAppId.Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId)).SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (existingGames.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.GameName == gameName && x.Host != null && x.Host.IsConnected))
            {
                client.Queue(new RT_MSG_SERVER_APP()
                {
                    Message = new MediusMatchCreateGameResponse()
                    {
                        MessageID = matchCreateGameRequest.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return;
            }

            // P2P Matchmaking.
            if (matchCreateGameRequest.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
            {
                // Create and add
                try
                {
                    // Try to get next free MPS server
                    // If none exist, return error to clist
                    //var dme = MediusStarter.ProxyServer.GetFreeDme(client.ApplicationId);
                    MPS mps = MediusClass.GetMPS();
                    if (mps == null)
                    {
                        client.Queue(new MediusMatchCreateGameResponse()
                        {
                            MessageID = matchCreateGameRequest.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusTransactionTimedOut
                        });
                        return;
                    }

                    ClientObject dme = client;

                    Game game = new(client, matchCreateGameRequest, dme);

                    await client.JoinGameP2P(game);
                    await AddGame(game);

                    mps.SendServerCreateGameWithAttributesRequestP2P(matchCreateGameRequest.MessageID.ToString(), client.AccountId, game.MediusWorldId, false, game, client);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(e);

                    // Failure creating match game for some reason
                    client.Queue(new MediusMatchCreateGameResponse()
                    {
                        MessageID = matchCreateGameRequest.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusMatchGameCreationFailed
                    });
                }
            }
            else
            //DME
            {
                // Try to get next free dme server
                // If none exist, return error to clist
                var dme = MediusClass.ProxyServer.GetFreeDme(client.ApplicationId, client.LocationId);
                if (dme == null)
                {
                    client.Queue(new MediusMatchCreateGameResponse()
                    {
                        MessageID = matchCreateGameRequest.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusTransactionTimedOut
                    });
                    return;
                }

                // Create and add
                try
                {
                    Game game = new(client, matchCreateGameRequest, dme);

                    await AddGame(game);

                    game.RequestData = matchCreateGameRequest.RequestData;
                    game.AppDataSize = matchCreateGameRequest.ApplicationDataSize;
                    game.AppData = matchCreateGameRequest.ApplicationData;

                    // Send create game request to dme server
                    dme.Queue(new MediusServerCreateGameWithAttributesRequest()
                    {
                        MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{matchCreateGameRequest.MessageID}-{0}"),
                        WorldID = game.MediusWorldId,
                        Attributes = game.Attributes,
                        ApplicationID = client.ApplicationId,
                        MaxClients = game.MaxPlayers
                    });
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(e);

                    // Failure adding game for some reason
                    client.Queue(new MediusMatchCreateGameResponse()
                    {
                        MessageID = matchCreateGameRequest.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusMatchGameCreationFailed
                    });
                }
            }
        }
        #endregion

        #region Create Game P2P

        #region MediusServerCreateGameOnMeRequest / MediusServerCreateGameOnSelfRequest / MediusServerCreateGameOnSelfRequest0
        public async Task<ClientObject?> CreateGameP2P(ClientObject client, IMediusRequest request, IChannel channel)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            int AccountId = -1;
            int WorldId = -1;
            string? gameName = null;
            NetAddressList gameNetAddressList = new();

            string p2pHostAddressRemoved = ((IPEndPoint)channel.RemoteAddress).Address.ToString().Remove(0, 7);

            if (request is MediusServerCreateGameOnMeRequest r)
            {
                AccountId = r.AccountID;
                WorldId = r.WorldID;
                gameName = r.GameName;

                if (r.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport ||
                        r.AddressList.AddressList[1].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                {
                    gameNetAddressList.AddressList[0].IPBinaryBitOne = r.AddressList.AddressList[0].IPBinaryBitOne;
                    gameNetAddressList.AddressList[0].IPBinaryBitTwo = r.AddressList.AddressList[0].IPBinaryBitTwo;
                    gameNetAddressList.AddressList[0].IPBinaryBitThree = r.AddressList.AddressList[0].IPBinaryBitThree;
                    gameNetAddressList.AddressList[0].IPBinaryBitFour = r.AddressList.AddressList[0].IPBinaryBitFour;
                    gameNetAddressList.AddressList[0].BinaryPort = r.AddressList.AddressList[0].BinaryPort;


                    gameNetAddressList.AddressList[1].IPBinaryBitOne = r.AddressList.AddressList[1].IPBinaryBitOne;
                    gameNetAddressList.AddressList[1].IPBinaryBitTwo = r.AddressList.AddressList[1].IPBinaryBitTwo;
                    gameNetAddressList.AddressList[1].IPBinaryBitThree = r.AddressList.AddressList[1].IPBinaryBitThree;
                    gameNetAddressList.AddressList[1].IPBinaryBitFour = r.AddressList.AddressList[1].IPBinaryBitFour;
                    gameNetAddressList.AddressList[1].BinaryPort = r.AddressList.AddressList[1].BinaryPort;
                }
                else
                    gameNetAddressList = r.AddressList;
            }
            else if (request is MediusServerCreateGameOnSelfRequest r1)
            {
                AccountId = r1.AccountID;
                WorldId = r1.WorldID;
                gameName = r1.GameName;
                gameNetAddressList = r1.AddressList;
            }
            else if (request is MediusServerCreateGameOnSelfRequest0 r2)
            {
                WorldId = r2.WorldID;
                gameName = r2.GameName;
                gameNetAddressList = r2.AddressList;
            }

            IEnumerable<Game> existingGames = _lookupsByAppId.Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId)).SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (existingGames.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.GameName == gameName && x.Host != null && x.Host.IsConnected))
            {
                client?.Queue(new RT_MSG_SERVER_APP()
                {
                    Message = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return null;
            }

            // Create and add game.
            try
            {
                ClientObject dme = client;

                if (request is not MediusServerCreateGameOnSelfRequest0)
                    client = GetClientByAccountId(AccountId, client.ApplicationId) ?? throw new Exception("[MumManager] - CreateGameP2P - Specified an invalid AccountId or requested client not yet logged-in!");

                Game game = new(client, request, dme);

                await client.JoinGameP2P(game);
                await AddGame(game);

                //Send Success response
                dme.Queue(new MediusServerCreateGameOnMeResponse()
                {
                    MessageID = request.MessageID,
                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                    MediusWorldID = game.MediusWorldId,
                });

                return client;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            // Failure adding game for some reason
            client.Queue(new MediusCreateGameResponse()
            {
                MessageID = request.MessageID,
                MediusWorldID = -1,
                StatusCode = MediusCallbackStatus.MediusFail
            });

            return null;
        }
        #endregion

        #endregion

        #region JoinGameRequest
        public Task JoinGame(ClientObject client, MediusJoinGameRequest request)
        {
            List<int> skipGameHostTypeCheckAppIds = new() { 10680, 10681, 10683, 10684, 10994 }; // Some 108 Medius games not send the same gameHostType in request, but still expect a result...
            List<int> approvedMaxPlayersAppIds = new() { 20624, 22500, 22920, 22924, 22930, 23360, 24000, 24180 };

            if ((client.ApplicationId == 20371 || client.ApplicationId == 20374) && client.ClientHomeData != null && client.ClientHomeData.VersionAsDouble >= 01.21)
                approvedMaxPlayersAppIds.Add(client.ApplicationId);

            Game? game = null;
            if (request.MediusWorldID == -1)
            {
                IEnumerable<Game>? gameList = GetGameListAppId(client.ApplicationId, 1, 100); // -1 means any?

                if (gameList == null)
                {
                    LoggerAccessor.LogWarn($"[MediusManager] - Join Game Request Handler Error: Error in retrieving game world info from MUM cache [{request.MediusWorldID}]");
                    client.Queue(new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusGameNotFound
                    });
                }
                else
                    game = gameList.FirstOrDefault();
            }
            else
                game = GetGameByGameId(request.MediusWorldID); // MUM original fetches GameWorldData

            if (game == null)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: Error in retrieving game world info from MUM cache [{request.MediusWorldID}]");
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusGameNotFound
                });
            }

            #region Password
            else if (!string.IsNullOrEmpty(game.GamePassword) && game.GamePassword != request.GamePassword)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This game's password {game.GamePassword} doesn't match the requested GamePassword {request.GamePassword}");
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusInvalidPassword
                });
            }
            #endregion

            #region MaxPlayers
            else if (game.PlayerCount >= game.MaxPlayers)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This game does not allow more than {game.MaxPlayers}. Current player count: {game.PlayerCount}");
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusWorldIsFull
                });
            }
            #endregion

            #region GameHostType check
            else if (!skipGameHostTypeCheckAppIds.Contains(client.ApplicationId) && request.GameHostType != game.GameHostType)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This games HostType {game.GameHostType} does not match the Requests HostType {request.GameHostType}");
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusRequestDenied
                });
            }
            #endregion

            #region JoinType.MediusJoinAsMassSpectator
            else if (request.JoinType == MediusJoinType.MediusJoinAsMassSpectator && (Convert.ToInt32(game.Attributes) & 2) == 0)
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This game does not allow mass spectators. Attributes: {game.Attributes}");

            #endregion

            else
            {
                _ = EnqueueJoinGameAsync(client.ApplicationId, game.MediusWorldId, () =>
                {
                    // if This is a Peer to Peer Player Host as DME we treat differently
                    if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                        && game.netAddressList?.AddressList?[0].AddressType == NetAddressType.NetAddressTypeSignalAddress)
                    {
                        game.Host?.Queue(new MediusServerJoinGameRequest()
                        {
                            MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                            ConnectInfo = new NetConnectionInfo()
                            {
                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                AccessKey = client.AccessToken,
                                SessionKey = client.SessionKey,
                                TargetWorldID = game.MediusWorldId,
                                ServerKey = MediusClass.GlobalAuthPublic,
                                AddressList = new NetAddressList()
                                {
                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                    {
                                        new NetAddress() { AddressBytes = request.AddressList.AddressList[0].AddressBytes, Port = request.AddressList.AddressList[0].Port, AddressType = NetAddressType.NetAddressTypeSignalAddress},
                                        new NetAddress() { AddressType = NetAddressType.NetAddressNone},
                                    }
                                },
                            }
                        });
                    }
                    else if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                            && game.netAddressList?.AddressList?[0].AddressType == NetAddressType.NetAddressTypeExternal
                            && game.netAddressList?.AddressList?[1].AddressType == NetAddressType.NetAddressTypeInternal)
                    {
                        game.Host?.Queue(new MediusServerJoinGameRequest()
                        {
                            MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                            ConnectInfo = new NetConnectionInfo()
                            {
                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                AccessKey = client.AccessToken,
                                SessionKey = client.SessionKey,
                                TargetWorldID = game.MediusWorldId,
                                ServerKey = MediusClass.GlobalAuthPublic,
                                AddressList = new NetAddressList()
                                {
                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                    {
                                    new NetAddress() { Address = request.AddressList?.AddressList?[0].Address, Port = request.AddressList.AddressList[0].Port, AddressType = NetAddressType.NetAddressTypeExternal},
                                    new NetAddress() { Address = request.AddressList?.AddressList?[1].Address, Port = request.AddressList.AddressList[1].Port, AddressType = NetAddressType.NetAddressTypeInternal},
                                    }
                                },
                            }
                        });
                    }

                    else if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                            && game.netAddressList?.AddressList?[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport
                            && game.netAddressList?.AddressList?[1].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                    {
                        game.Host?.Queue(new MediusServerJoinGameRequest()
                        {
                            MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}"),
                            ConnectInfo = new NetConnectionInfo()
                            {
                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                AccessKey = client.AccessToken,
                                SessionKey = client.SessionKey,
                                TargetWorldID = game.MediusWorldId,
                                ServerKey = MediusClass.GlobalAuthPublic,
                                AddressList = new NetAddressList()
                                {
                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                    {
                                    new NetAddress() { IPBinaryBitOne = request.AddressList.AddressList[0].IPBinaryBitOne,
                                        IPBinaryBitTwo = request.AddressList.AddressList[0].IPBinaryBitTwo,
                                        IPBinaryBitThree = request.AddressList.AddressList[0].IPBinaryBitThree,
                                        IPBinaryBitFour = request.AddressList.AddressList[0].IPBinaryBitFour,
                                        BinaryPort = request.AddressList.AddressList[0].BinaryPort,
                                        AddressType = NetAddressType.NetAddressTypeBinaryExternalVport},
                                    new NetAddress() { IPBinaryBitOne = request.AddressList.AddressList[1].IPBinaryBitOne,
                                        IPBinaryBitTwo = request.AddressList.AddressList[1].IPBinaryBitTwo,
                                        IPBinaryBitThree = request.AddressList.AddressList[1].IPBinaryBitThree,
                                        IPBinaryBitFour = request.AddressList.AddressList[1].IPBinaryBitFour,
                                        BinaryPort = request.AddressList.AddressList[1].BinaryPort,
                                        AddressType = NetAddressType.NetAddressTypeBinaryInternalVport},
                                    }
                                },
                            }
                        });

                    }

                    // Else send normal Connection type to DME
                    else
                    {
                        ClientObject? dme = game.DMEServer;

                        if (client.MediusVersion > 108 && client.ApplicationId != 10994 || client.ApplicationId == 10680 || client.ApplicationId == 10681 || client.ApplicationId == 10683 || client.ApplicationId == 10684)
                            dme?.Queue(new MediusServerJoinGameRequest()
                            {
                                MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                                ConnectInfo = new NetConnectionInfo()
                                {
                                    Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP,
                                    TargetWorldID = game.MediusWorldId,
                                    AccessKey = client.AccessToken,
                                    SessionKey = client.SessionKey,
                                    ServerKey = MediusClass.GlobalAuthPublic
                                }
                            });
                        else
                            dme?.Queue(new MediusServerJoinGameRequest()
                            {
                                MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                                ConnectInfo = new NetConnectionInfo()
                                {
                                    Type = NetConnectionType.NetConnectionTypeClientServerTCP,
                                    TargetWorldID = game.MediusWorldId,
                                    AccessKey = client.AccessToken,
                                    SessionKey = client.SessionKey,
                                    ServerKey = MediusClass.GlobalAuthPublic
                                }
                            });
                    }

                    return Task.CompletedTask;
                });
            }

            return Task.CompletedTask;
        }
        #endregion

        #region JoinGameRequest0
        public void JoinGame0(ClientObject client, MediusJoinGameRequest0 request)
        {
            List<int> approvedMaxPlayersAppIds = new() { 20624, 22500, 22920, 22924, 22930, 23360, 24000, 24180 };

            if ((client.ApplicationId == 20371 || client.ApplicationId == 20374) && client.ClientHomeData != null && client.ClientHomeData.VersionAsDouble >= 01.21)
                approvedMaxPlayersAppIds.Add(client.ApplicationId);

            Game? game = GetGameByGameId(request.MediusWorldID);
            if (game == null)
            {
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusGameNotFound
                });
            }
            else if (!string.IsNullOrEmpty(game.GamePassword) && game.GamePassword != request.GamePassword)
            {
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusInvalidPassword
                });
            }
            else if (game.PlayerCount >= game.MaxPlayers)
            {
                client.Queue(new MediusJoinGameResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusWorldIsFull
                });
            }
            else
            {
                _ = EnqueueJoinGame0Async(client.ApplicationId, game.MediusWorldId, () =>
                {
                    // if This is a Peer to Peer Player Host as DME we treat differently
                    if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
                    {
                        game.Host?.Queue(new MediusServerJoinGameRequest()
                        {
                            MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                            ConnectInfo = new NetConnectionInfo()
                            {
                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                TargetWorldID = game.MediusWorldId,
                                AccessKey = client.AccessToken,
                                SessionKey = client.SessionKey,
                                ServerKey = MediusClass.GlobalAuthPublic
                            }
                        });
                    }
                    // Else send normal Connection type
                    else
                    {
                        game.DMEServer?.Queue(new MediusServerJoinGameRequest()
                        {
                            MessageID = new MessageId($"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"),
                            ConnectInfo = new NetConnectionInfo()
                            {
                                Type = NetConnectionType.NetConnectionTypeClientServerTCP,
                                TargetWorldID = game.MediusWorldId,
                                AccessKey = client.AccessToken,
                                SessionKey = client.SessionKey,
                                ServerKey = MediusClass.GlobalAuthPublic
                            }
                        });
                    }

                    return Task.CompletedTask;
                });
            }
        }

        private async Task EnqueueJoinGameAsync(int applicationId, int gameId, Func<Task> taskFunc)
        {
            SemaphoreSlim semaphore = _gameJoinSemaphores.GetOrAdd((applicationId, gameId), _ => new SemaphoreSlim(1, 1));

            ConcurrentQueue<Func<Task>> queue = _gameJoinRequestQueue.GetOrAdd((applicationId, gameId), _ => new ConcurrentQueue<Func<Task>>());

            Task enqueueJob = Task.Run(() => { queue.Enqueue(taskFunc); });

            if (!semaphore.Wait(0))
                return;
            else
                await enqueueJob.ConfigureAwait(false);

            try
            {
                while (queue.TryDequeue(out var taskToExecute))
                {
                    try
                    {
                        await taskToExecute().ConfigureAwait(false);
                    }
                    catch
                    {
                    }
                    
                    await Task.Delay(gameJoinDelay).ConfigureAwait(false);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task EnqueueJoinGame0Async(int applicationId, int gameId, Func<Task> taskFunc)
        {
            SemaphoreSlim semaphore = _gameJoin0Semaphores.GetOrAdd((applicationId, gameId), _ => new SemaphoreSlim(1, 1));

            ConcurrentQueue<Func<Task>> queue = _gameJoin0RequestQueue.GetOrAdd((applicationId, gameId), _ => new ConcurrentQueue<Func<Task>>());

            Task enqueueJob = Task.Run(() => { queue.Enqueue(taskFunc); });

            if (!semaphore.Wait(0))
                return;
            else
                await enqueueJob.ConfigureAwait(false);

            try
            {
                while (queue.TryDequeue(out var taskToExecute))
                {
                    try
                    {
                        await taskToExecute().ConfigureAwait(false);
                    }
                    catch
                    {
                    }

                    await Task.Delay(gameJoinDelay).ConfigureAwait(false);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        #endregion

        #endregion

        #region Channels

        public List<Channel> GetAllChannels()
        {
            List<Channel> channels = new();

            foreach (int[] appIds in _appIdGroups.Values)
            {
                foreach (int appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out QuickLookup? quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(quickLookup.AppIdToChannel
                                .Where(pair => pair.Key == appId)
                                .SelectMany(pair => pair.Value)
                                .ToList());
                        }
                    }
                }
            }

            return channels;
        }

        public List<Channel> GetAllChannels(int appId)
        {
            List<Channel> channels = new();

            if (_lookupsByAppId.TryGetValue(appId, out QuickLookup? quickLookup))
            {
                lock (quickLookup.AppIdToChannel)
                {
                    channels.AddRange(quickLookup.AppIdToChannel
                        .Where(pair => pair.Key == appId)
                        .SelectMany(pair => pair.Value)
                        .ToList());
                }
            }

            return channels;
        }

        public Channel? GetChannelByChannelId(int channelId, int appId)
        {
            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        Channel? channel = quickLookup.AppIdToChannel.SelectMany(kv => kv.Value).Where(c => c.Id == channelId && c.ApplicationId == appId).FirstOrDefault();
                        if (channel != null)
                            return channel;
                    }
                }
            }

            return null;
        }

        public Channel? GetChannelByChannelName(string channelName, int appId)
        {
            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        Channel? channel = quickLookup.AppIdToChannel.SelectMany(kv => kv.Value).FirstOrDefault(x => x.Name == channelName && x.ApplicationId == appId);
                        if (channel != null)
                            return channel;
                    }
                }
            }

            return null;
        }

        public Channel? GetChannelByRequestFilter(int appId, ChannelType type, ulong FieldMask1, ulong FieldMask2, ulong FieldMask3, ulong FieldMask4, MediusLobbyFilterMaskLevelType filterMaskLevelType)
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.Type == type &&
                    x.ApplicationId == appId &&
                    x.GenericField1 == FieldMask1 &&
                    x.GenericField2 == FieldMask2 &&
                    x.GenericField3 == FieldMask3 &&
                    x.GenericField4 == FieldMask4 &&
                    x.GenericFieldLevel == (MediusWorldGenericFieldLevelType)filterMaskLevelType)
                .First();
        }

        public uint GetChannelCount(ChannelType type, int appId)
        {
            uint count = 0;

            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        count += (uint)quickLookup.AppIdToChannel.SelectMany(kv => kv.Value).Count(x => x.Type == type && x.ApplicationId == appId);
                    }
                }
            }

            return count;
        }

        public Channel GetOrCreateDefaultLobbyChannel(int appId, int MediusVersion)
        {
            Channel? channel = null;

            foreach (int appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out QuickLookup? quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        channel = quickLookup.AppIdToChannel.SelectMany(kv => kv.Value).FirstOrDefault(x => x.ApplicationId == appId && x.Id == 1);
                        if (channel != null)
                            return channel;
                    }
                }
            }

            // create default
            channel = Channel.GetDefaultChannel(appId, MediusVersion);

            _ = AddChannel(channel);

            return channel;
        }

        public async Task AddChannel(Channel channel)
        {
            if (!_lookupsByAppId.TryGetValue(channel.ApplicationId, out QuickLookup? quickLookup))
                _lookupsByAppId.TryAdd(channel.ApplicationId, quickLookup = new QuickLookup());

            lock (quickLookup.AppIdToChannel)
            {
                if (!quickLookup.AppIdToChannel.ContainsKey(channel.ApplicationId))
                    quickLookup.AppIdToChannel.Add(channel.ApplicationId, new List<Channel>() { channel });
                else
                    quickLookup.AppIdToChannel[channel.ApplicationId].Add(channel);
            }

            await channel.OnChannelCreate(channel).ConfigureAwait(false);
        }

        /// <summary>
        /// Filter Worlds by AppId, and if set by client WorldFilters
        /// </summary>
        /// <param name="appId">ApplicationId</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="type"></param>
        /// <param name="FieldMask1"></param>
        /// <param name="FieldMask2"></param>
        /// <param name="FieldMask3"></param>
        /// <param name="FieldMask4"></param>
        /// <param name="filterMaskLevelType"></param>
        /// <returns></returns>
        public IEnumerable<Channel> GetChannelListFiltered(int appId, int pageIndex, int pageSize, ChannelType type, ulong FieldMask1, ulong FieldMask2, ulong FieldMask3, ulong FieldMask4, MediusLobbyFilterMaskLevelType filterMaskLevelType)
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.Type == type &&
                    x.ApplicationId == appId &&
                    x.GenericField1 == FieldMask1 &&
                    x.GenericField2 == FieldMask2 &&
                    x.GenericField3 == FieldMask3 &&
                    x.GenericField4 == FieldMask4 &&
                    x.GenericFieldLevel == (MediusWorldGenericFieldLevelType)filterMaskLevelType)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Filter Worlds by AppId
        /// </summary>
        /// <param name="appId">ApplicationId</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Channel> GetChannelListUnfiltered(int appId, int pageIndex, int pageSize)
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.ApplicationId == appId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Filter Worlds by AppId
        /// </summary>
        /// <param name="appId">ApplicationId</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Channel> GetChannelList(int appId, int pageIndex, int pageSize, ChannelType type)
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.Type == type &&
                    x.ApplicationId == appId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Filter Worlds by AppId
        /// </summary>
        /// <param name="appId">ApplicationId</param>
        /// <returns></returns>
        public Channel GetChannelLeastPoplated(int appId)
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.ApplicationId == appId).OrderBy(kvp => kvp.PlayerCount).First();
        }
        #endregion

        #region Party

        public Task joinParty(ClientObject client, MediusPartyJoinByIndexRequest request)
        {
            List<int> approvedMaxPlayersAppIds = new() { 20624, 22500, 22920, 22924, 22930, 23360, 24000, 24180 };

            Party? party = GetPartyByPartyId(request.MediusWorldID); // MUM original fetches GameWorldData
            if (party == null)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: Error in retrieving party info from MUM cache [{request.MediusWorldID}]");

                client.Queue(new MediusPartyJoinByIndexResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusNoResult
                });
            }
            #region Password
            else if (party.PartyPassword != null && party.PartyPassword != string.Empty && party.PartyPassword != request.PartyPassword)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This party's password {party.PartyPassword} doesn't match the requested party Password {request.PartyPassword}");
                client.Queue(new MediusPartyJoinByIndexResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusInvalidPassword
                });
            }
            #endregion
            #region MaxPlayers
            else if (party.PlayerCount >= party.MaxPlayers)
            {
                LoggerAccessor.LogWarn($"Join Game Request Handler Error: This game does not allow more than {party.MaxPlayers}. Current player count: {party.PlayerCount}");
                client.Queue(new MediusPartyJoinByIndexResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusWorldIsFull
                });
            }
            #endregion
            #region GameHostType check
            else if (request.PartyHostType != party.PartyHostType)
            {
                LoggerAccessor.LogWarn($"PartyJoinByIndex Request Handler Error: This party's HostType {party.PartyHostType} does not match the Requests HostType {request.PartyHostType}");
                client.Queue(new MediusPartyJoinByIndexResponse()
                {
                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                    MessageID = request.MessageID,
                    StatusCode = MediusCallbackStatus.MediusRequestDenied
                });
            }
            #endregion
            else
            {
                var dme = party.DMEServer;

                dme?.Queue(new MediusServerJoinGameRequest()
                {
                    MessageID = new MessageId($"{party.MediusWorldId}-{client.AccountId}-{request.MessageID}-{1}"),
                    ConnectInfo = new NetConnectionInfo()
                    {
                        Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP,
                        TargetWorldID = party.MediusWorldId,
                        AccessKey = client.AccessToken,
                        SessionKey = client.SessionKey,
                        ServerKey = request.pubKey
                    }
                });
            }

            return Task.CompletedTask;
        }

        public async Task AddParty(Party party)
        {
            if (!_lookupsByAppId.TryGetValue(party.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(party.ApplicationId, quickLookup = new QuickLookup());

            quickLookup.PartyIdToGame.Add(party.MediusWorldId, party);
            await HorizonServerConfiguration.Database.CreateParty(party.ToPartyDTO());
        }

        public async Task CreateParty(ClientObject client, IMediusRequest request)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            var appIdsInGroup = GetAppIdsInGroup(client.ApplicationId);
            string? partyName = null;
            if (request is MediusPartyCreateRequest r)
                partyName = r.PartyName;

            var existingParties = _lookupsByAppId.Where(x => appIdsInGroup.Contains(client.ApplicationId)).SelectMany(x => x.Value.PartyIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (existingParties.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.PartyName == partyName && x.Host != null && x.Host.IsConnected))
            {
                client.Queue(new RT_MSG_SERVER_APP()
                {
                    Message = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return;
            }

            // Try to get next free dme server
            // If none exist, return error to clist
            var dme = MediusClass.ProxyServer.GetFreeDme(client.ApplicationId, client.LocationId);
            MPS mps = MediusClass.GetMPS();
            if (mps == null)
            {
                
            }
            else if (dme == null)
            {
                client.Queue(new MediusPartyCreateResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusTransactionTimedOut
                });
                return;
            }
            else
            {
                // Create and add party.
                try
                {
                    Party? party = new(client, request, dme);
                    await AddParty(party);

                    await client.JoinParty(party, party.MediusWorldId);

                    mps.SendServerCreateGameWithAttributesRequest(request.MessageID.ToString(), client.AccountId, party.GameChannel!.Id, party.MediusWorldId, true, (int)party.Attributes, client.ApplicationId, party.MaxPlayers);

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(e);
                }
            }

            // Failure adding game for some reason
            client.Queue(new MediusPartyCreateResponse()
            {
                MessageID = request.MessageID,
                MediusWorldID = -1,
                StatusCode = MediusCallbackStatus.MediusFail
            });
        }

        #endregion

        #region MFS
        public IEnumerable<MediusFile> GetFilesList(string path, string filenameBeginsWith, uint pageSize, uint startingEntryNumber, int appId)
        {
            lock (_mediusFiles)
            {

                string[]? files = null;
                int counter = 0;
                /*
                if (appId == 11203 || appId == 10994 || appId == 11204)
                {

                    string JakXPath = path + filenameBeginsWith.Split("*");
                } else
                {

                }
                */
                if (filenameBeginsWith.ToString() == "*")
                {
                    files = Directory.GetFiles(path).Select(file => Path.GetFileName(file)).ToArray();
                }
                else
                {
                    files = Directory.GetFiles(path, Convert.ToString(filenameBeginsWith));
                }

                if (files.Length < pageSize)
                {
                    counter = files.Count();
                }
                else
                {
                    counter = (int)pageSize - 1;
                }

                for (int i = (int)startingEntryNumber - 1; i < counter; i++)
                {
                    string fileName = files[i];
                    FileInfo fi = new FileInfo(fileName);

                    try
                    {
                        _mediusFiles.Add(new MediusFile()
                        {
                            FileName = files[i],
                            FileID = i,
                            FileSize = (int)fi.Length,
                            CreationTimeStamp = (int)fi.CreationTime.ToUnixTimeU32(),
                        });
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogWarn($"MFS FileList Exception:\n{e}");
                    }
                }
                return _mediusFiles;
            }
        }

        public IEnumerable<MediusFile> GetFilesListExt(string path, string filenameBeginsWith, uint pageSize, uint startingEntryNumber, int appId)
        {
            lock (_mediusFiles)
            {
                if (startingEntryNumber == 0)
                    return _mediusFiles;

                int counter = 0;

                //JakX FileNameBeginsWith requires extra handling for folders
                if (appId == 11203 || appId == 10994 || appId == 11204)
                {

                    string JakXPath = path + filenameBeginsWith.Split("*");

                    string[] filesArray = Directory.GetFiles(JakXPath);

                    //files = Directory.GetFiles(path).Select(file => Path.GetFileName(filenameBeginsWith)).ToArray();

                    if (filesArray.Length < pageSize)
                    {
                        counter = filesArray.Count() - 1;
                    }
                    else
                    {
                        counter = (int)pageSize - 1;
                    }

                    for (int i = (int)(startingEntryNumber - 1); i < counter; i++)
                    {
                        //string[] pathArray = path.TrimStart('[').TrimEnd(']').Split(',');

                        //string[] fileName = filesArray[i].Split(path, path.Length, options: StringSplitOptions.None);
                        //string FileNameAppended = UsingStringJoin(fileName);

                        try
                        {
                            string fileName = filesArray[i];
                            FileInfo fi = new FileInfo(fileName);

                            LoggerAccessor.LogInfo($"Medius Files Count: {_mediusFiles.Count}");

                            _mediusFiles.Add(new MediusFile()
                            {
                                FileName = fileName.ToString(),
                                FileID = i,
                                FileSize = (int)fi.Length,
                                CreationTimeStamp = (int)fi.CreationTime.ToUnixTimeU32(),
                            });
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogWarn($"MFS FileListExt Exception:\n{e}");
                        }
                    }
                    return _mediusFiles;
                }
                else
                {

                    string filenameBeginsWithAppended = filenameBeginsWith.Remove(filenameBeginsWith.Length - 1);


                    string[] filesArray = Directory.GetFiles(path);

                    //files = Directory.GetFiles(path).Select(file => Path.GetFileName(filenameBeginsWith)).ToArray();

                    if (filesArray.Length < pageSize)
                    {
                        counter = filesArray.Count() - 1;
                    }
                    else
                    {
                        counter = (int)pageSize - 1;
                    }

                    for (int i = (int)(startingEntryNumber - 1); i < counter; i++)
                    {
                        //string[] pathArray = path.TrimStart('[').TrimEnd(']').Split(',');

                        //string[] fileName = filesArray[i].Split(path, path.Length, options: StringSplitOptions.None);
                        //string FileNameAppended = UsingStringJoin(fileName);

                        try
                        {
                            string fileName = filesArray[i];
                            FileInfo fi = new FileInfo(fileName);

                            _mediusFiles.Where(x => x.FileName == fileName.StartsWith(filenameBeginsWithAppended).ToString()).ToList();

                            /*
                            _mediusFiles.Add(new MediusFile()
                            {
                                FileName = filenameBeginsWithAppended.ToString(),
                                FileID = (uint)i,
                                FileSize = (uint)fi.Length,
                                CreationTimeStamp = Utils.ToUnixTime(fi.CreationTime),
                            });
                            */
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogWarn($"MFS FileListExt Exception:\n{e}");
                        }
                    }
                    return _mediusFiles;
                }


            }
        }

        public IEnumerable<MediusFileMetaData> UpdateFileMetaData(string path, int appId, MediusFile mediusFile, MediusFileMetaData mediusFileMetaData)

        {
            lock (_mediusFilesToUpdateMetaData)
            {

                /*
                if (filename.ToString() != null)
                {
                    files = Directory.GetFiles(path).Select(file => Path.GetFileName(file)).ToArray();
                }
                else
                {
                    files = Directory.GetFiles(path, Convert.ToString(filename));
                }
                */
                try
                {
                    _mediusFilesToUpdateMetaData.Add(new MediusFileMetaData()
                    {
                        Key = mediusFileMetaData.Key,
                        Value = mediusFileMetaData.Value,
                    });
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MFS] - UpdateMetaData Exception:\n{e}");
                }

                return _mediusFilesToUpdateMetaData;
            }
        }

        public Task UploadMediusFile(MediusFileUploadResponse fileUploadResponse, ClientObject clientObject)
        {

            var uploadState = clientObject.Upload;

            if (fileUploadResponse.iXferStatus >= MediusFileXferStatus.End)
                return Task.CompletedTask;

            try
            {
                LoggerAccessor.LogInfo($"[MFS] - Bytes Received Total [{uploadState.BytesReceived}] < [{uploadState.TotalSize}]");
                uploadState.Stream.Seek(fileUploadResponse.iStartByteIndex, SeekOrigin.Begin);
                uploadState.Stream.Write(fileUploadResponse.Data, 0, fileUploadResponse.iDataSize);
                uploadState.BytesReceived += fileUploadResponse.iDataSize;
                uploadState.PacketNumber++;

                if (uploadState.BytesReceived < uploadState.TotalSize)
                {
                    clientObject.Queue(new MediusFileUploadServerRequest()
                    {
                        MessageID = fileUploadResponse.MessageID,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        iPacketNumber = uploadState.PacketNumber,
                        iReqStartByteIndex = uploadState.BytesReceived,
                        iXferStatus = MediusFileXferStatus.Mid
                    });
                }
                else
                {
                    clientObject.Queue(new MediusFileUploadServerRequest()
                    {
                        MessageID = fileUploadResponse.MessageID,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        iPacketNumber = uploadState.PacketNumber,
                        iReqStartByteIndex = uploadState.BytesReceived,
                        iXferStatus = MediusFileXferStatus.End
                    });
                }
            }
            catch
            {
                clientObject.Queue(new MediusFileUploadServerRequest()
                {
                    MessageID = fileUploadResponse.MessageID,
                    StatusCode = MediusCallbackStatus.MediusFileInternalAccessError,
                    iPacketNumber = uploadState.PacketNumber,
                    iReqStartByteIndex = uploadState.BytesReceived,
                    iXferStatus = MediusFileXferStatus.Error
                });
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Buddies

        public List<AccountDTO> AddToBuddyInvitations(int appId, AccountDTO accountToAdd)
        {
            var appIdsInGroup = GetAppIdsInGroup(appId);

            if (!_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                _lookupsByAppId.TryAdd(appId, quickLookup = new QuickLookup());

            lock (quickLookup.BuddyInvitationsToClient)
            {
                quickLookup.BuddyInvitationsToClient.Add(accountToAdd.AccountId, accountToAdd);
            }

            return _lookupsByAppId.Where(x => appIdsInGroup.Contains(x.Key))
                .SelectMany(x => x.Value.BuddyInvitationsToClient.Select(x => x.Value))
                .ToList();
        }

        public List<AccountDTO> GetBuddyInvitations(int appId, int AccountId)
        {
            var appIdsInGroup = GetAppIdsInGroup(appId);

            return _lookupsByAppId.Where(x => appIdsInGroup.Contains(x.Key))
                .SelectMany(x => x.Value.BuddyInvitationsToClient.Select(x => x.Value))
                .Where(x => x.AccountId == AccountId)
                .ToList();
        }

        #endregion

        #region Clans

        //public Clan GetClanByAccountId(int clanId, int appId)
        //{
        //    if (_clanIdToClan.TryGetValue(clanId, out var result))
        //        return result;

        //    return null;
        //}

        //public Clan GetClanByAccountName(string clanName, int appId)
        //{
        //    clanName = clanName.ToLower();
        //    if (_clanNameToClan.TryGetValue(clanName, out var result))
        //        return result;

        //    return null;
        //}

        //public void AddClan(Clan clan)
        //{
        //    if (!_lookupsByAppId.TryGetValue(clan.ApplicationId, out var quickLookup))
        //        _lookupsByAppId.TryAdd(dmeClient.ApplicationId, quickLookup = new QuickLookup());

        //    _clanNameToClan.Add(clan.Name.ToLower(), clan);
        //    _clanIdToClan.Add(clan.Id, clan);
        //}

        #endregion

        #region Tick

        public async Task Tick()
        {
            await TickClients();

            await TickChannels();

            await TickGames();
        }

        private async Task TickChannels()
        {
            try
            {
                // Tick channels
                foreach (var quickLookup in _lookupsByAppId)
                {
                    foreach (var channelKeyPair in quickLookup.Value.AppIdToChannel)
                    {
                        List<Channel> channelsToRemove = new();

                        foreach (Channel channel in channelKeyPair.Value)
                        {
                            if (channel.ReadyToDestroy)
                                channelsToRemove.Add(channel);
                            else
                                await channel.Tick();
                        }

                        lock (quickLookup.Value.AppIdToChannel)
                        {
                            foreach (Channel channel in channelsToRemove)
                            {
                                LoggerAccessor.LogWarn($"Destroying Channel {channel}");

                                channelKeyPair.Value.Remove(channel);

                                Channel.UnregisterId(channelKeyPair.Key, channel.Id);
                            }

                            RoomManager.UpdateRoomsFromChannels(channelKeyPair.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MediusManager] - Error in TickChannels {ex}");
            }
        }

        private async Task TickGames()
        {
            try
            {
                Queue<(QuickLookup, int)> gamesToRemove = new();
                Queue<(QuickLookup, int)> partiesToRemove = new();

                // Tick games
                foreach (var quickLookup in _lookupsByAppId)
                {
                    int appid = quickLookup.Key;
                    List<int> gameKeysToUpdate = new List<int>();
                    List<int> partyKeysToUpdate = new List<int>();

                    foreach (var gameKeyPair in quickLookup.Value.GameIdToGame)
                    {
                        var game = gameKeyPair.Value;

                        if (game.MediusWorldId != gameKeyPair.Key)
                        {
                            LoggerAccessor.LogWarn($"Game Id mismatch: key {gameKeyPair.Key} != game.MediusWorldId {game.MediusWorldId}, updating.");
                            gameKeysToUpdate.Add(gameKeyPair.Key);
                        }
                        if (game.ReadyToDestroy)
                        {
                            LoggerAccessor.LogWarn($"Destroying Game {game}");
                            await game.EndGame(appid);
                            gamesToRemove.Enqueue((quickLookup.Value, gameKeyPair.Key));
                        }
                        else if (game.Destroyed)
                        {
                            LoggerAccessor.LogWarn($"Removing destroyed Game {game}");
                            gamesToRemove.Enqueue((quickLookup.Value, gameKeyPair.Key));
                        }
                        else
                            await game.Tick();
                    }

                    lock (quickLookup.Value.GameIdToGame)
                    {
                        foreach (var previousKey in gameKeysToUpdate)
                        {
                            var game = quickLookup.Value.GameIdToGame[previousKey];
                            quickLookup.Value.GameIdToGame.Remove(previousKey);
                            quickLookup.Value.GameIdToGame[game.MediusWorldId] = game;
                        }
                    }

                    foreach (var partyKeyPair in quickLookup.Value.PartyIdToGame)
                    {
                        var party = partyKeyPair.Value;

                        if (party.MediusWorldId != partyKeyPair.Key)
                        {
                            LoggerAccessor.LogWarn($"Party Id mismatch: key {partyKeyPair.Key} != party.MediusWorldId {party.MediusWorldId}, updating.");
                            partyKeysToUpdate.Add(partyKeyPair.Key);
                        }
                        if (party.ReadyToDestroy)
                        {
                            LoggerAccessor.LogWarn($"Destroying Party {party}");
                            await party.EndParty(appid);
                            partiesToRemove.Enqueue((quickLookup.Value, partyKeyPair.Key));
                        }
                        else if (party.Destroyed)
                        {
                            LoggerAccessor.LogWarn($"Removing destroyed Party {party}");
                            partiesToRemove.Enqueue((quickLookup.Value, partyKeyPair.Key));
                        }
                        else
                            await party.Tick();
                    }

                    lock (quickLookup.Value.PartyIdToGame)
                    {
                        foreach (var previousKey in partyKeysToUpdate)
                        {
                            var party = quickLookup.Value.PartyIdToGame[previousKey];
                            quickLookup.Value.PartyIdToGame.Remove(previousKey);
                            quickLookup.Value.PartyIdToGame[party.MediusWorldId] = party;
                        }
                    }
                }

                // Remove games
                while (gamesToRemove.TryDequeue(out var lookupAndGameId))
                    lookupAndGameId.Item1.GameIdToGame.Remove(lookupAndGameId.Item2);

                while (partiesToRemove.TryDequeue(out var lookupAndGameId))
                    lookupAndGameId.Item1.PartyIdToGame.Remove(lookupAndGameId.Item2);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MediusManager] - Error in TickGames {ex}");
            }
        }

        private async Task TickClients()
        {
            try
            {
                Queue<(int, string)> clientsToRemove = new();

                while (_addQueue.TryDequeue(out ClientObject? newClient))
                {
                    if (!_lookupsByAppId.TryGetValue(newClient.ApplicationId, out var quickLookup))
                        _lookupsByAppId.TryAdd(newClient.ApplicationId, quickLookup = new QuickLookup());

                    try
                    {
                        if (newClient.IsLoggedIn)
                        {
                            quickLookup.AccountIdToClient.Add(newClient.AccountId, newClient);
							
                            if (!string.IsNullOrEmpty(newClient.AccountName))
                                quickLookup.AccountNameToClient.Add(newClient.AccountName.ToLower(), newClient);
                        }

                        if (!string.IsNullOrEmpty(newClient.AccessToken))
                            quickLookup.AccessTokenToClient.Add(newClient.AccessToken, newClient);

                        if (!string.IsNullOrEmpty(newClient.SessionKey))
                            quickLookup.SessionKeyToClient.Add(newClient.SessionKey, newClient);
                    }
                    catch (Exception e)
                    {
                        // clean up
                        if (newClient != null)
                        {
                            if (newClient.IsLoggedIn)
                            {
                                quickLookup.AccountIdToClient.Remove(newClient.AccountId);
								
                                if (!string.IsNullOrEmpty(newClient.AccountName))
                                    quickLookup.AccountNameToClient.Remove(newClient.AccountName.ToLower());
                            }

                            if (!string.IsNullOrEmpty(newClient.AccessToken))
                                quickLookup.AccessTokenToClient.Remove(newClient.AccessToken);

                            if (!string.IsNullOrEmpty(newClient.SessionKey))
                                quickLookup.SessionKeyToClient.Remove(newClient.SessionKey);
                        }

                        LoggerAccessor.LogError(e);
                        //throw e;
                    }
                }

                foreach (var quickLookup in _lookupsByAppId)
                {
                    foreach (var clientKeyPair in quickLookup.Value.SessionKeyToClient)
                    {
                        if (!clientKeyPair.Value.IsConnected)
                        {
                            LoggerAccessor.LogWarn($"Destroying Client {clientKeyPair.Value}");

                            // end server session and Logout
                            await clientKeyPair.Value.Logout();
                            clientKeyPair.Value.EndServerSession();

                            clientsToRemove.Enqueue((quickLookup.Key, clientKeyPair.Key));
                        }
                        else if (clientKeyPair.Value.Timedout)
                            clientKeyPair.Value.ForceDisconnect();
                    }
                }

                // Remove
                while (clientsToRemove.TryDequeue(out (int, string) appIdAndSessionKey))
                {
                    if (_lookupsByAppId.TryGetValue(appIdAndSessionKey.Item1, out QuickLookup? quickLookup))
                    {
                        if (quickLookup.SessionKeyToClient.Remove(appIdAndSessionKey.Item2, out ClientObject? clientObject))
                        {
                            if (!string.IsNullOrEmpty(clientObject.AccessToken))
                                quickLookup.AccessTokenToClient.Remove(clientObject.AccessToken);

                            quickLookup.AccountIdToClient.Remove(clientObject.AccountId);

                            if (!string.IsNullOrEmpty(clientObject.AccountName))
                               quickLookup.AccountNameToClient.Remove(clientObject.AccountName.ToLower());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MediusManager] - Error in TickClients {ex}");
            }
        }

        #endregion

        #region App Ids

        public async Task OnDatabaseAuthenticated()
        {
            // get supported app ids
            AppIdDTO[]? appids = await HorizonServerConfiguration.Database.GetAppIds();

            if (appids != null)
                // build dictionary of app ids from response
                _appIdGroups = appids.ToDictionary(x => x.Name, x => x.AppIds != null ? x.AppIds.ToArray() : Array.Empty<int>());
        }

        public bool IsAppIdSupported(int appId)
        {
            return _appIdGroups.Any(x => x.Value.Contains(appId));
        }

        public int[] GetAppIdsInGroup(int appId)
        {
            return _appIdGroups.FirstOrDefault(x => x.Value.Contains(appId)).Value ?? Array.Empty<int>();
        }

        #endregion

        #region Misc

        public SECURITY_MODE GetServerSecurityMode(SECURITY_MODE securityMode, RSA_KEY rsaKey)
        {
            int result;

            result = (int)securityMode;

            if (securityMode == SECURITY_MODE.MODE_UNKNOWN)
            {
                //result = (KM_GetLocalPublicKey(RSA_KEY, 0x80000000, 0) != 0) + 1;

                //securityMode = (SECURITY_MODE)result;
            }


            return (SECURITY_MODE)result;
        }

        public void rt_msg_server_check_protocol_compatibility(int clientVersion, byte p_compatible)
        {



        }

        #region AnonymouseAccountIdGenerator
        /// <summary>
        /// Generates a Random Anonymous AccountID for MediusAnonymouseAccountRequest <br></br>
        /// Or if one doesn't exist in Database
        /// </summary>
        /// <param name="AnonymousIDRangeSeed">Config Value for changing the MAS</param>
        /// <returns></returns>
        public int AnonymousAccountIDGenerator(int AnonymousIDRangeSeed)
        {
            // Anonymous login expect a negative id < 0.
            return new Random().Next(-80000000, AnonymousIDRangeSeed);
        }
        #endregion

        public string UsingStringJoin(string[] array)
        {
            return string.Join(string.Empty, array);
        }

        #endregion

        #region Vulgarity

        public void load_classifier(string filename)
        {
            int classifier = 0;
            var rootPath = Path.GetFullPath(MediusClass.Settings.MediusVulgarityRootPath);

            try
            {
                var stream = File.Open(rootPath + filename, FileMode.OpenOrCreate, FileAccess.Read);

                FileInfo fi = new FileInfo(filename);

                if (fi.Extension == "cl")
                    classifier = read_classifier(stream);
                else
                    LoggerAccessor.LogWarn($"Unknown file type in {rootPath}.\n");

            }
            catch (Exception)
            {
                LoggerAccessor.LogWarn($"Cannot open {rootPath + "/" + filename}.\n");
            }

        }

        public int read_classifier(FileStream fileClassifier)
        {
            if (fileClassifier.Length == 20398493)
            {

            }

            return 0;
        }
        #endregion

        #region DmeServerClient

        public Task DmeServerClientIpQuery(int WorldID, int TargetClient, IPAddress IP)
        {

            return Task.CompletedTask;
        }

        public DME_SERVER_RESULT DmeServerMapRtError(uint RtError)
        {
            DME_SERVER_RESULT result;
            if (RtError == 52518)
            {
                result = DME_SERVER_RESULT.DME_SERVER_UNKNOWN_MSG_TYPE;
                return result;
            }

            if (RtError > 0xCD26)
            {
                result = DME_SERVER_RESULT.DME_SERVER_MUTEX_ERROR;
                if (RtError != 52528)
                {
                    if (RtError > 0xCD30)
                    {
                        result = DME_SERVER_RESULT.DME_SERVER_UNSECURED_ERROR;
                        if (RtError != 52533)
                        {
                            if (RtError > 0xCD35)
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_CONFIG_ERROR;
                                if (RtError != 52535)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_BUFF_OVERFLOW_ERROR;
                                    if (RtError >= 0xCD37)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_PARTIAL_RW_ERROR;
                                        if (RtError != 52536)
                                        {
                                            result = DME_SERVER_RESULT.DME_SERVER_CLIENT_ALREADY_DISCONNECTED;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //if (RtError == 52530)
                                //return 30;
                                result = DME_SERVER_RESULT.DME_SERVER_NO_MORE_WORLDS;
                                if (RtError >= 0xCD32)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_CLIENT_LIMIT;
                                    if (RtError != 52531)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_ENCRYPTED_ERROR;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = DME_SERVER_RESULT.DME_SERVER_MSG_TOO_BIG;
                        if (RtError != 52523)
                        {
                            if (RtError > 0xCD2B)
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_PARTIAL_WRITE;
                                if (RtError != 52525)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_UNKNOWN_MSG_TYPE;
                                    if (RtError >= 0xCD2D)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_SOCKET_RESET_ERROR;
                                        if (RtError != 52526)
                                        {
                                            result = DME_SERVER_RESULT.DME_SERVER_CIRC_BUF_ERROR;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_TCP_GET_WORLD_INDEX;
                                if (RtError != 52520)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_WOULD_BLOCK;
                                    if (RtError >= 0xCD28)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_READ_ERROR;
                                        if (RtError != 52521)
                                        {
                                            result = DME_SERVER_RESULT.DME_SERVER_SOCKET_CLOSED;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (RtError == 52508)
                {
                    result = DME_SERVER_RESULT.DME_SERVER_CONN_MSG_ERROR;
                    return result;
                }
                if (RtError > 0xCD1C)
                {
                    result = DME_SERVER_RESULT.DME_SERVER_SOCKET_BIND_ERROR;
                    if (RtError != 52513)
                    {
                        if (RtError > 0xCD21)
                        {
                            result = DME_SERVER_RESULT.DME_SERVER_SOCKET_LISTEN_ERROR;
                            if (RtError != 52515)
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_SOCKET_POLL_ERROR;
                                if (RtError >= 0xCD23)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_SOCKET_READ_ERROR;
                                    if (RtError != 52516)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_SOCKET_WRITE_ERROR;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = DME_SERVER_RESULT.DME_SERVER_STACK_LOAD_ERROR;
                            if (RtError != 52510)
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_WORLD_FULL;
                                if (RtError >= 0xCD1E)
                                {
                                    result = DME_SERVER_RESULT.DME_SERVER_SOCKET_CREATE_ERROR;
                                    if (RtError != 52511)
                                    {
                                        result = DME_SERVER_RESULT.DME_SERVER_SOCKET_OPT_ERROR;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (RtError == 52503)
                    {
                        result = DME_SERVER_RESULT.DME_SERVER_NOT_INITIALIZED;
                        return result;
                    }

                    if (RtError <= 0xCD17)
                    {
                        if (RtError == 52501)
                        {
                            result = DME_SERVER_RESULT.DME_SERVER_INVALID_PARAM;
                            return result;
                        }
                        if (RtError > 0xCD15)
                        {
                            result = DME_SERVER_RESULT.DME_SERVER_NOT_IMPLEMENTED;
                            return result;
                        }
                    }

                    result = DME_SERVER_RESULT.DME_SERVER_MEM_ALLOC;
                    if (RtError != 52505)
                    {
                        result = DME_SERVER_RESULT.DME_SERVER_UNKNOWN_RESULT;
                        if (RtError >= 0xCD19)
                        {
                            result = DME_SERVER_RESULT.DME_SERVER_SOCKET_LIMIT;
                            if (RtError != 52506)
                            {
                                result = DME_SERVER_RESULT.DME_SERVER_UNKNOWN_CONN_ERROR;
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Matchmaking
        public int CalculateSizeOfMatchRoster(MediusMatchRosterInfo roster)
        {
            int rosterSize;
            uint v3;
            uint partySize;

            MediusMatchPartyInfo mediusMatchPartyInfo = new MediusMatchPartyInfo();

            if (roster == null)
                return 0;
            rosterSize = 4 * roster.NumParties + 8;
            partySize = (uint)roster.Parties;
            v3 = (uint)(4 * roster.NumParties + partySize - 4);
            while (partySize <= v3)
            {
                rosterSize += CalculateSizeOfMatchParty(mediusMatchPartyInfo);
                partySize += 4;
            }

            return rosterSize;
        }

        public int CalculateSizeOfMatchParty(MediusMatchPartyInfo party)
        {
            int MatchPartySize;
            if (party != null)
            {
                MatchPartySize = 8 * party.NumPlayers + 8;
            }
            else
            {
                MatchPartySize = 0;
            }

            return MatchPartySize;
        }
        #endregion
    }
}
