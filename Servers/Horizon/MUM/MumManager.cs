using System.Collections.Concurrent;
using System.Net;
using CustomLogger;
using Horizon.HTTPSERVICE;
using Horizon.LIBRARY.Database.Models;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using Prometheus;
using IChannel = DotNetty.Transport.Channels.IChannel;

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

        public static Counter playersJoined = Metrics.CreateCounter(
            "medius_players_joined_total",
            "Total number of players having joined Medius."
        );
        public static Counter channelsCreated = Metrics.CreateCounter(
            "medius_channels_created_total",
            "Total number of created channels in Medius."
        );
        private static readonly Counter gamesCreated = Metrics.CreateCounter(
            "medius_games_created_total",
            "Total number of created games in Medius."
        );

        private const int gameJoinDelay = 2500;

        private Dictionary<string, int[]> _appIdGroups = new();

        private readonly ConcurrentDictionary<(int, int), SemaphoreSlim> _gameJoinSemaphores =
            new();
        private readonly ConcurrentDictionary<(int, int), SemaphoreSlim> _gameJoin0Semaphores =
            new();

        private readonly ConcurrentDictionary<
            (int, int),
            ConcurrentQueue<Func<Task>>
        > _gameJoinRequestQueue = new();
        private readonly ConcurrentDictionary<
            (int, int),
            ConcurrentQueue<Func<Task>>
        > _gameJoin0RequestQueue = new();

        private readonly ConcurrentDictionary<int, QuickLookup> _lookupsByAppId = new();

        private readonly List<MediusFile> _mediusFiles = new();
        private readonly List<MediusFileMetaData> _mediusFilesToUpdateMetaData = new();

        private readonly ConcurrentQueue<ClientObject> _addQueue = new();

        #region Clients
        public List<ClientObject> GetClients(int appId)
        {
            return appId == 0
                ? _lookupsByAppId
                    .SelectMany(x => x.Value.SessionKeyToClient.Select(x => x.Value))
                    .ToList()
                : _lookupsByAppId
                    .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                    .SelectMany(x => x.Value.SessionKeyToClient.Select(x => x.Value))
                    .ToList();
        }

        public ClientObject? GetClientByAccountId(int accountId, int appId)
        {
            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    if (quickLookup.AccountIdToClient.TryGetValue(accountId, out var result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientByAccountName(string accountName, int appId)
        {
            accountName = accountName.ToLower();

            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    if (quickLookup.AccountNameToClient.TryGetValue(accountName, out var result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientByAccessToken(string? accessToken, int appId)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    if (quickLookup.AccessTokenToClient.TryGetValue(accessToken, out var result))
                        return result;
                }
            }

            return null;
        }

        public ClientObject? GetClientBySessionKey(string? sessionKey, int appId)
        {
            if (string.IsNullOrEmpty(sessionKey))
                return null;

            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    if (quickLookup.SessionKeyToClient.TryGetValue(sessionKey, out var result))
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
                LoggerAccessor.LogError(
                    "[MumManager] - Trying to add a LoggedIn client but client is not logged in!"
                );
                return;
            }

            foreach (var appIdInGroup in GetAppIdsInGroup(newClient.ApplicationId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(newClient.AccountName))
                        {
                            var accountNameLower = newClient.AccountName.ToLower();

                            if (
                                !quickLookup.AccountIdToClient.TryAdd(
                                    newClient.AccountId,
                                    newClient
                                )
                            )
                                quickLookup.AccountIdToClient[newClient.AccountId] = newClient;
                            if (
                                !quickLookup.AccountNameToClient.TryAdd(accountNameLower, newClient)
                            )
                                quickLookup.AccountNameToClient[accountNameLower] = newClient;
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

                        LoggerAccessor.LogError(
                            $"[MumManager] - Error in AddOrUpdateLoggedInClient {e}"
                        );
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
                        count += (uint)quickLookup.GameIdToGame.Count;
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
                    var game = lookupByAppId
                        .Value.GameIdToGame.FirstOrDefault(x =>
                            x.Value?.DMEServer?.SessionKey == dmeSessionKey
                            && x.Value?.MediusWorldId == MediusWorldId
                        )
                        .Value;
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
                    var party = lookupByAppId
                        .Value.PartyIdToGame.FirstOrDefault(x =>
                            x.Value?.DMEServer?.SessionKey == dmeSessionKey
                            && x.Value?.MediusWorldId == MediusWorldId
                        )
                        .Value;
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
                    var party = lookupByAppId
                        .Value.PartyIdToGame.FirstOrDefault(x =>
                            x.Value?.ApplicationId == appId && x.Value.PartyName == name
                        )
                        .Value;
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
                    var channel = lookupByAppId
                        .Value.AppIdToChannel.SelectMany(kv => kv.Value) // Flatten all channels
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
                    if (lookupByAppId.Value.GameIdToGame.TryGetValue(gameId, out var game))
                        return game;
                }
            }

            return null;
        }

        public IEnumerable<Game> GetAllGamesByAppId(int applicationId)
        {
            if (_lookupsByAppId.TryGetValue(applicationId, out var quickLookup))
            {
                lock (quickLookup.GameIdToGame)
                {
                    foreach (var pair in quickLookup.GameIdToGame)
                    {
                        if (
                            pair.Value.GameChannel != null
                            && pair.Value.GameChannel.ApplicationId == applicationId
                        )
                            yield return pair.Value;
                    }
                }
            }
        }

        public List<Game> GetGamesByGameIdViaChannelFilter(int gameId)
        {
            List<Channel> channels = new();
            List<Game> Games = new();

            foreach (var appIds in _appIdGroups.Values)
            {
                foreach (var appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(
                                quickLookup
                                    .AppIdToChannel.Where(pair => pair.Key == appId)
                                    .SelectMany(pair => pair.Value)
                                    .ToList()
                            );
                        }
                    }
                }
            }

            foreach (var channel in channels)
            {
                if (channel.Game != null && channel.Game.MediusWorldId == gameId)
                    Games.Add(channel.Game);
            }

            return Games;
        }

        public List<Game> GetGamesByGameNameViaChannelFilter(string gameName)
        {
            List<Channel> channels = new();
            List<Game> Games = new();

            foreach (var appIds in _appIdGroups.Values)
            {
                foreach (var appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(
                                quickLookup
                                    .AppIdToChannel.Where(pair => pair.Key == appId)
                                    .SelectMany(pair => pair.Value)
                                    .ToList()
                            );
                        }
                    }
                }
            }

            foreach (var channel in channels)
            {
                if (channel.Game != null && channel.Game.GameName == gameName)
                    Games.Add(channel.Game);
            }

            return Games;
        }

        public Party? GetPartyByPartyId(int partyId)
        {
            foreach (var lookupByAppId in _lookupsByAppId)
            {
                lock (lookupByAppId.Value.PartyIdToGame)
                {
                    if (lookupByAppId.Value.PartyIdToGame.TryGetValue(partyId, out var party))
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
            await HorizonServerConfiguration
                .Database.CreateGame(game.ToGameDTO())
                .ConfigureAwait(false);

            gamesCreated.Inc();
        }

        public int GetGameCountAppId(int appId)
        {
            if (!_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                _lookupsByAppId.TryAdd(appId, quickLookup = new QuickLookup());

            return quickLookup.GameIdToGame.Count;
        }

        public IEnumerable<Game> GetGameList(
            int appId,
            int pageIndex,
            int pageSize,
            IEnumerable<GameListFilter> filters
        )
        {
#if DEBUG
            return _lookupsByAppId
                .Where(x =>
                {
                    var condition = GetAppIdsInGroup(appId).Contains(x.Key);
                    if (!condition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameList - Condition failed: AppId {x.Key} is not in the group."
                        );
                    return condition;
                })
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                {
                    var worldStatusCondition =
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed;
                    if (!worldStatusCondition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameList - Condition failed: WorldStatus is {x.WorldStatus}."
                        );

                    var filtersCondition = !filters.Any() || filters.All(y => y.IsMatch(x));
                    if (!filtersCondition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameList - Condition failed: Filters do not match."
                        );

                    return worldStatusCondition && filtersCondition;
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
#else
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                    (
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed
                    ) && (!filters.Any() || filters.All(y => y.IsMatch(x)))
                )
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
#endif
        }

        public IEnumerable<Game> GetGameListOnAnyMatchingFilter(
            int appId,
            int pageIndex,
            int pageSize,
            IEnumerable<GameListFilter> filters
        )
        {
#if DEBUG
            return _lookupsByAppId
                .Where(x =>
                {
                    var condition = GetAppIdsInGroup(appId).Contains(x.Key);
                    if (!condition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: AppId {x.Key} is not in the group."
                        );
                    return condition;
                })
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                {
                    var worldStatusCondition =
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed;
                    if (!worldStatusCondition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: WorldStatus is {x.WorldStatus}."
                        );

                    var filtersCondition = !filters.Any() || filters.Any(y => y.IsMatch(x));
                    if (!filtersCondition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameListOnAnyMatchingFilter - Condition failed: No filters matched."
                        );

                    return worldStatusCondition && filtersCondition;
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
#else
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                    (
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed
                    ) && (!filters.Any() || filters.Any(y => y.IsMatch(x)))
                )
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
#endif
        }

        public IEnumerable<Game> GetGameListAppId(int appId, int pageIndex, int pageSize)
        {
#if DEBUG
            return _lookupsByAppId
                .Where(x =>
                {
                    var condition = GetAppIdsInGroup(appId).Contains(x.Key);
                    if (!condition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameListAppId - Condition failed: AppId {x.Key} is not in the group."
                        );
                    return condition;
                })
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                {
                    var worldStatusCondition =
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed;
                    if (!worldStatusCondition)
                        LoggerAccessor.LogWarn(
                            $"[MumManager] - DEBUG - GetGameListAppId - Condition failed: WorldStatus is {x.WorldStatus}."
                        );

                    return worldStatusCondition;
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
#else
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.GameIdToGame.Select(x => x.Value))
                .Where(x =>
                    (
                        x.WorldStatus == MediusWorldStatus.WorldActive
                        || x.WorldStatus == MediusWorldStatus.WorldStaging
                        || x.WorldStatus == MediusWorldStatus.WorldClosed
                    )
                )
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

            var existingGames = _lookupsByAppId
                .Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId))
                .SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (
                existingGames.Any(x =>
                    x.WorldStatus != MediusWorldStatus.WorldClosed
                    && x.WorldStatus != MediusWorldStatus.WorldInactive
                    && x.GameName == gameName
                    && x.Host != null
                    && x.Host.IsConnected
                )
            )
            {
                client.Queue(
                    new RT_MSG_SERVER_APP()
                    {
                        Message = new MediusCreateGameResponse()
                        {
                            MessageID = request.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusGameNameExists,
                        },
                    }
                );
                return;
            }

            // Try to get next free dme server
            // If none exist, return error to clist
            var dme = Program.MediusManager?.ProxyServer.GetFreeDme(
                client.ApplicationId,
                client.LocationId
            );

            if (dme == null)
            {
                client.Queue(
                    new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                    }
                );
                return;
            }
            else
            {
                // Create and add game.
                try
                {
                    game = new Game(client, request, dme);

                    await AddGame(game).ConfigureAwait(false);

                    // Send create game request to dme server
                    dme.Queue(
                        new MediusServerCreateGameWithAttributesRequest()
                        {
                            MessageID = new MessageId(
                                $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                            ),
                            WorldID = game.GameChannel!.Id,
                            Attributes = game.Attributes,
                            ApplicationID = client.ApplicationId,
                            MaxClients = game.MaxPlayers,
                        }
                    );

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in CreateGame {e}");
                }
            }

            // Failure adding game for some reason
            client.Queue(
                new MediusCreateGameResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusFail,
                }
            );
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

            var existingGames = _lookupsByAppId
                .Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId))
                .SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (
                existingGames.Any(x =>
                    x.WorldStatus != MediusWorldStatus.WorldClosed
                    && x.WorldStatus != MediusWorldStatus.WorldInactive
                    && x.GameName == gameName
                    && x.Host != null
                    && x.Host.IsConnected
                )
            )
            {
                client.Queue(
                    new RT_MSG_SERVER_APP()
                    {
                        Message = new MediusCreateGameResponse()
                        {
                            MessageID = request.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusGameNameExists,
                        },
                    }
                );
                return;
            }

            // Try to get next free dme server
            // If none exist, return error to clist
            var dme = Program.MediusManager?.ProxyServer.GetFreeDme(
                client.ApplicationId,
                client.LocationId
            );

            if (dme == null)
            {
                client.Queue(
                    new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                    }
                );
                return;
            }
            else
            {
                // Create and add game.
                try
                {
                    Game game = new(client, request, dme);

                    await AddGame(game).ConfigureAwait(false);

                    // Send create game request to dme server
                    dme.Queue(
                        new MediusServerCreateGameWithAttributesRequest()
                        {
                            MessageID = new MessageId(
                                $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                            ),
                            WorldID = game.GameChannel!.Id,
                            Attributes = game.Attributes,
                            ApplicationID = client.ApplicationId,
                            MaxClients = game.MaxPlayers,
                        }
                    );

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in CreateGame1 {e}");
                }
            }

            // Failure adding game for some reason
            client.Queue(
                new MediusCreateGameResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusFail,
                }
            );
        }
        #endregion

        #region MatchCreateGame
        public async Task MatchCreateGame(
            ClientObject client,
            MediusMatchCreateGameRequest matchCreateGameRequest,
            IChannel channel
        )
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            string? gameName = null;
            if (matchCreateGameRequest is MediusMatchCreateGameRequest r)
                gameName = r.GameName;

            var existingGames = _lookupsByAppId
                .Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId))
                .SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (
                existingGames.Any(x =>
                    x.WorldStatus != MediusWorldStatus.WorldClosed
                    && x.WorldStatus != MediusWorldStatus.WorldInactive
                    && x.GameName == gameName
                    && x.Host != null
                    && x.Host.IsConnected
                )
            )
            {
                client.Queue(
                    new RT_MSG_SERVER_APP()
                    {
                        Message = new MediusMatchCreateGameResponse()
                        {
                            MessageID = matchCreateGameRequest.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusGameNameExists,
                        },
                    }
                );
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
                    var mps = Program.MediusManager?.ProxyServer;
                    if (mps == null)
                    {
                        client.Queue(
                            new MediusMatchCreateGameResponse()
                            {
                                MessageID = matchCreateGameRequest.MessageID,
                                MediusWorldID = -1,
                                StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                            }
                        );
                        return;
                    }

                    var dme = client;

                    Game game = new(client, matchCreateGameRequest, dme);

                    await client.JoinGameP2P(game).ConfigureAwait(false);

                    await AddGame(game).ConfigureAwait(false);

                    mps.SendServerCreateGameOrPartyWithAttributesRequestP2P(
                        matchCreateGameRequest.MessageID.ToString(),
                        client.AccountId,
                        game.MediusWorldId,
                        game,
                        client
                    );

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in MatchCreateGame P2P {e}");
                }
            }
            else
            //DME
            {
                // Try to get next free dme server
                // If none exist, return error to clist
                var dme = Program.MediusManager?.ProxyServer.GetFreeDme(
                    client.ApplicationId,
                    client.LocationId
                );
                if (dme == null)
                {
                    client.Queue(
                        new MediusMatchCreateGameResponse()
                        {
                            MessageID = matchCreateGameRequest.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                        }
                    );
                    return;
                }

                // Create and add
                try
                {
                    Game game = new(client, matchCreateGameRequest, dme);

                    await AddGame(game).ConfigureAwait(false);

                    game.RequestData = matchCreateGameRequest.RequestData;
                    game.AppDataSize = matchCreateGameRequest.ApplicationDataSize;
                    game.AppData = matchCreateGameRequest.ApplicationData;

                    // Send create game request to dme server
                    dme.Queue(
                        new MediusServerCreateGameWithAttributesRequest()
                        {
                            MessageID = new MessageId(
                                $"{game.MediusWorldId}-{client.AccountId}-{matchCreateGameRequest.MessageID}-{0}"
                            ),
                            WorldID = game.GameChannel!.Id,
                            Attributes = game.Attributes,
                            ApplicationID = client.ApplicationId,
                            MaxClients = game.MaxPlayers,
                        }
                    );

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in MatchCreateGame DME {e}");
                }
            }

            // Failure creating match game for some reason
            client.Queue(
                new MediusMatchCreateGameResponse()
                {
                    MessageID = matchCreateGameRequest.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusMatchGameCreationFailed,
                }
            );
        }
        #endregion

        #region Create Game P2P

        #region MediusServerCreateGameOnMeRequest / MediusServerCreateGameOnSelfRequest / MediusServerCreateGameOnSelfRequest0
        public async Task<ClientObject?> CreateGameP2P(
            ClientObject client,
            IMediusRequest request,
            IChannel channel
        )
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            var AccountId = -1;
            var WorldId = -1;
            string? gameName = null;
            NetAddressList gameNetAddressList = new();

            var p2pHostAddressRemoved = ((IPEndPoint)channel.RemoteAddress)
                .Address.ToString()
                .Remove(0, 7);

            if (request is MediusServerCreateGameOnMeRequest r)
            {
                AccountId = r.AccountID;
                WorldId = r.WorldID;
                gameName = r.GameName;

                if (
                    r.AddressList.AddressList[0].AddressType
                        == NetAddressType.NetAddressTypeBinaryExternalVport
                    || r.AddressList.AddressList[1].AddressType
                        == NetAddressType.NetAddressTypeBinaryInternalVport
                )
                {
                    gameNetAddressList.AddressList[0].IPBinaryBitOne = r.AddressList
                        .AddressList[0]
                        .IPBinaryBitOne;
                    gameNetAddressList.AddressList[0].IPBinaryBitTwo = r.AddressList
                        .AddressList[0]
                        .IPBinaryBitTwo;
                    gameNetAddressList.AddressList[0].IPBinaryBitThree = r.AddressList
                        .AddressList[0]
                        .IPBinaryBitThree;
                    gameNetAddressList.AddressList[0].IPBinaryBitFour = r.AddressList
                        .AddressList[0]
                        .IPBinaryBitFour;
                    gameNetAddressList.AddressList[0].BinaryPort = r.AddressList
                        .AddressList[0]
                        .BinaryPort;

                    gameNetAddressList.AddressList[1].IPBinaryBitOne = r.AddressList
                        .AddressList[1]
                        .IPBinaryBitOne;
                    gameNetAddressList.AddressList[1].IPBinaryBitTwo = r.AddressList
                        .AddressList[1]
                        .IPBinaryBitTwo;
                    gameNetAddressList.AddressList[1].IPBinaryBitThree = r.AddressList
                        .AddressList[1]
                        .IPBinaryBitThree;
                    gameNetAddressList.AddressList[1].IPBinaryBitFour = r.AddressList
                        .AddressList[1]
                        .IPBinaryBitFour;
                    gameNetAddressList.AddressList[1].BinaryPort = r.AddressList
                        .AddressList[1]
                        .BinaryPort;
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

            var existingGames = _lookupsByAppId
                .Where(x => GetAppIdsInGroup(client.ApplicationId).Contains(client.ApplicationId))
                .SelectMany(x => x.Value.GameIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (
                existingGames.Any(x =>
                    x.WorldStatus != MediusWorldStatus.WorldClosed
                    && x.WorldStatus != MediusWorldStatus.WorldInactive
                    && x.GameName == gameName
                    && x.Host != null
                    && x.Host.IsConnected
                )
            )
            {
                client?.Queue(
                    new RT_MSG_SERVER_APP()
                    {
                        Message = new MediusCreateGameResponse()
                        {
                            MessageID = request.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusGameNameExists,
                        },
                    }
                );
                return null;
            }

            // Create and add game.
            try
            {
                var dme = client;

                if (request is not MediusServerCreateGameOnSelfRequest0)
                    client =
                        GetClientByAccountId(AccountId, client.ApplicationId)
                        ?? throw new Exception(
                            "[MumManager] - CreateGameP2P - Specified an invalid AccountId or requested client not yet logged-in!"
                        );

                Game game = new(client, request, dme);

                await client.JoinGameP2P(game).ConfigureAwait(false);

                await AddGame(game).ConfigureAwait(false);

                //Send Success response
                dme.Queue(
                    new MediusServerCreateGameOnMeResponse()
                    {
                        MessageID = request.MessageID,
                        Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                        MediusWorldID = game.MediusWorldId,
                    }
                );

                return client;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[MumManager] - Error in CreateGameP2P {e}");
            }

            // Failure adding game for some reason
            client.Queue(
                new MediusCreateGameResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusFail,
                }
            );

            return null;
        }
        #endregion

        #endregion

        #region JoinGameRequest
        public Task JoinGame(ClientObject client, MediusJoinGameRequest request)
        {
            List<int> skipGameHostTypeCheckAppIds = new() { 10680, 10681, 10683, 10684, 10994 }; // Some 108 Medius games not send the same gameHostType in request, but still expect a result...
            List<int> approvedMaxPlayersAppIds = new()
            {
                20624,
                22500,
                22920,
                22924,
                22930,
                23360,
                24000,
                24180,
            };

            if (
                (client.ApplicationId == 20371 || client.ApplicationId == 20374)
                && client.ClientHomeData != null
                && client.ClientHomeData.VersionAsDouble >= 01.21
            )
                approvedMaxPlayersAppIds.Add(client.ApplicationId);

            Game? game = null;
            if (request.MediusWorldID == -1)
            {
                var gameList = GetGameListAppId(client.ApplicationId, 1, 100); // -1 means any?

                if (gameList == null)
                {
                    LoggerAccessor.LogWarn(
                        $"[MumManager] - Join Game Request Handler Error: Error in retrieving game list info from MUM cache [{request.MediusWorldID}]"
                    );
                    client.Queue(
                        new MediusJoinGameResponse()
                        {
                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                            MessageID = request.MessageID,
                            StatusCode = MediusCallbackStatus.MediusGameNotFound,
                        }
                    );
                }
                else
                    game = gameList.FirstOrDefault();
            }
            else
                game = GetGameByGameId(request.MediusWorldID); // MUM original fetches GameWorldData

            if (game == null)
            {
                LoggerAccessor.LogWarn(
                    $"[MumManager] - Join Game Request Handler Error: Error in retrieving game world info from MUM cache [{request.MediusWorldID}]"
                );
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusGameNotFound,
                    }
                );
            }
            #region Password
            else if (
                !string.IsNullOrEmpty(game.GamePassword)
                && game.GamePassword != request.GamePassword
            )
            {
                LoggerAccessor.LogWarn(
                    $"[MumManager] - Join Game Request Handler Error: This game's password {game.GamePassword} doesn't match the requested GamePassword {request.GamePassword}"
                );
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusInvalidPassword,
                    }
                );
            }
            #endregion

            #region WorldStatus
            else if (game.WorldStatus == MediusWorldStatus.WorldClosed)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusRequestDenied,
                    }
                );
            }
            #endregion

            #region MaxPlayers
            else if (game.PlayerCount >= game.MaxPlayers)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusWorldIsFull,
                    }
                );
            }
            #endregion

            #region GameHostType check
            else if (
                !skipGameHostTypeCheckAppIds.Contains(client.ApplicationId)
                && request.GameHostType != game.GameHostType
            )
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusRequestDenied,
                    }
                );
            }
            #endregion

            #region JoinType.MediusJoinAsMassSpectator
            else if (
                request.JoinType == MediusJoinType.MediusJoinAsMassSpectator
                && (Convert.ToInt32(game.Attributes) & 2) == 0
            )
                LoggerAccessor.LogWarn(
                    $"[MumManager] - Join Game Request Handler Error: This game does not allow mass spectators. Attributes: {game.Attributes}"
                );
            #endregion

            else
            {
                _ = EnqueueJoinGameAsync(
                    client.ApplicationId,
                    game.MediusWorldId,
                    () =>
                    {
                        // if This is a Peer to Peer Player Host as DME we treat differently
                        if (
                            game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                            && game.netAddressList?.AddressList?[0].AddressType
                                == NetAddressType.NetAddressTypeSignalAddress
                        )
                        {
                            game.Host?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        TargetWorldID = game.MediusWorldId,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                        AddressList = new NetAddressList()
                                        {
                                            AddressList = new NetAddress[
                                                Constants.NET_ADDRESS_LIST_COUNT
                                            ]
                                            {
                                                new()
                                                {
                                                    AddressBytes = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .AddressBytes,
                                                    Port = request.AddressList.AddressList[0].Port,
                                                    AddressType =
                                                        NetAddressType.NetAddressTypeSignalAddress,
                                                },
                                                new()
                                                {
                                                    AddressType = NetAddressType.NetAddressNone,
                                                },
                                            },
                                        },
                                    },
                                }
                            );
                        }
                        else if (
                            game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                            && game.netAddressList?.AddressList?[0].AddressType
                                == NetAddressType.NetAddressTypeExternal
                            && game.netAddressList?.AddressList?[1].AddressType
                                == NetAddressType.NetAddressTypeInternal
                        )
                        {
                            game.Host?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        TargetWorldID = game.MediusWorldId,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                        AddressList = new NetAddressList()
                                        {
                                            AddressList = new NetAddress[
                                                Constants.NET_ADDRESS_LIST_COUNT
                                            ]
                                            {
                                                new()
                                                {
                                                    Address = request
                                                        .AddressList
                                                        ?.AddressList
                                                        ?[0]
                                                        .Address,
                                                    Port = request.AddressList.AddressList[0].Port,
                                                    AddressType =
                                                        NetAddressType.NetAddressTypeExternal,
                                                },
                                                new()
                                                {
                                                    Address = request
                                                        .AddressList
                                                        ?.AddressList
                                                        ?[1]
                                                        .Address,
                                                    Port = request.AddressList.AddressList[1].Port,
                                                    AddressType =
                                                        NetAddressType.NetAddressTypeInternal,
                                                },
                                            },
                                        },
                                    },
                                }
                            );
                        }
                        else if (
                            game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer
                            && game.netAddressList?.AddressList?[0].AddressType
                                == NetAddressType.NetAddressTypeBinaryExternalVport
                            && game.netAddressList?.AddressList?[1].AddressType
                                == NetAddressType.NetAddressTypeBinaryInternalVport
                        )
                        {
                            game.Host?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        TargetWorldID = game.MediusWorldId,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                        AddressList = new NetAddressList()
                                        {
                                            AddressList = new NetAddress[
                                                Constants.NET_ADDRESS_LIST_COUNT
                                            ]
                                            {
                                                new()
                                                {
                                                    IPBinaryBitOne = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .IPBinaryBitOne,
                                                    IPBinaryBitTwo = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .IPBinaryBitTwo,
                                                    IPBinaryBitThree = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .IPBinaryBitThree,
                                                    IPBinaryBitFour = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .IPBinaryBitFour,
                                                    BinaryPort = request
                                                        .AddressList
                                                        .AddressList[0]
                                                        .BinaryPort,
                                                    AddressType =
                                                        NetAddressType.NetAddressTypeBinaryExternalVport,
                                                },
                                                new()
                                                {
                                                    IPBinaryBitOne = request
                                                        .AddressList
                                                        .AddressList[1]
                                                        .IPBinaryBitOne,
                                                    IPBinaryBitTwo = request
                                                        .AddressList
                                                        .AddressList[1]
                                                        .IPBinaryBitTwo,
                                                    IPBinaryBitThree = request
                                                        .AddressList
                                                        .AddressList[1]
                                                        .IPBinaryBitThree,
                                                    IPBinaryBitFour = request
                                                        .AddressList
                                                        .AddressList[1]
                                                        .IPBinaryBitFour,
                                                    BinaryPort = request
                                                        .AddressList
                                                        .AddressList[1]
                                                        .BinaryPort,
                                                    AddressType =
                                                        NetAddressType.NetAddressTypeBinaryInternalVport,
                                                },
                                            },
                                        },
                                    },
                                }
                            );
                        }
                        // Else send normal Connection type to DME
                        else
                        {
                            var dme = game.DMEServer;

                            if (
                                (client.MediusVersion > 108 && client.ApplicationId != 10994)
                                || client.ApplicationId == 10680
                                || client.ApplicationId == 10681
                                || client.ApplicationId == 10683
                                || client.ApplicationId == 10684
                            )
                                dme?.Queue(
                                    new MediusServerJoinGameRequest()
                                    {
                                        MessageID = new MessageId(
                                            $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                        ),
                                        ConnectInfo = new NetConnectionInfo()
                                        {
                                            Type =
                                                NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP,
                                            TargetWorldID = game.MediusWorldId,
                                            AccessKey = client.AccessToken,
                                            SessionKey = client.SessionKey,
                                            ServerKey = new RSA_KEY(
                                                LIBRARY
                                                    .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                    .Reverse()
                                                    .ToArray()
                                            ),
                                        },
                                    }
                                );
                            else
                                dme?.Queue(
                                    new MediusServerJoinGameRequest()
                                    {
                                        MessageID = new MessageId(
                                            $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                        ),
                                        ConnectInfo = new NetConnectionInfo()
                                        {
                                            Type =
                                                NetConnectionType.NetConnectionTypeClientServerTCP,
                                            TargetWorldID = game.MediusWorldId,
                                            AccessKey = client.AccessToken,
                                            SessionKey = client.SessionKey,
                                            ServerKey = new RSA_KEY(
                                                LIBRARY
                                                    .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                    .Reverse()
                                                    .ToArray()
                                            ),
                                        },
                                    }
                                );
                        }

                        return Task.CompletedTask;
                    }
                );
            }

            return Task.CompletedTask;
        }
        #endregion

        #region JoinGameRequest0
        public void JoinGame0(ClientObject client, MediusJoinGameRequest0 request)
        {
            List<int> approvedMaxPlayersAppIds = new()
            {
                20624,
                22500,
                22920,
                22924,
                22930,
                23360,
                24000,
                24180,
            };

            if (
                (client.ApplicationId == 20371 || client.ApplicationId == 20374)
                && client.ClientHomeData != null
                && client.ClientHomeData.VersionAsDouble >= 01.21
            )
                approvedMaxPlayersAppIds.Add(client.ApplicationId);

            var game = GetGameByGameId(request.MediusWorldID);
            if (game == null)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusGameNotFound,
                    }
                );
            }
            else if (
                !string.IsNullOrEmpty(game.GamePassword)
                && game.GamePassword != request.GamePassword
            )
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusInvalidPassword,
                    }
                );
            }
            else if (game.WorldStatus == MediusWorldStatus.WorldClosed)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusRequestDenied,
                    }
                );
            }
            else if (game.PlayerCount >= game.MaxPlayers)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusWorldIsFull,
                    }
                );
            }
            else
            {
                _ = EnqueueJoinGame0Async(
                    client.ApplicationId,
                    game.MediusWorldId,
                    () =>
                    {
                        // if This is a Peer to Peer Player Host as DME we treat differently
                        if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
                        {
                            game.Host?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                        TargetWorldID = game.MediusWorldId,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                    },
                                }
                            );
                        }
                        // Else send normal Connection type
                        else
                        {
                            game.DMEServer?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{game.MediusWorldId}-{client.AccountId}-{request.MessageID}-{0}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypeClientServerTCP,
                                        TargetWorldID = game.MediusWorldId,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                    },
                                }
                            );
                        }

                        return Task.CompletedTask;
                    }
                );
            }
        }

        private async Task EnqueueJoinGameAsync(int applicationId, int gameId, Func<Task> taskFunc)
        {
            var semaphore = _gameJoinSemaphores.GetOrAdd(
                (applicationId, gameId),
                _ => new SemaphoreSlim(1, 1)
            );

            var queue = _gameJoinRequestQueue.GetOrAdd(
                (applicationId, gameId),
                _ => new ConcurrentQueue<Func<Task>>()
            );

            var enqueueJob = Task.Run(() =>
            {
                queue.Enqueue(taskFunc);
            });

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
                    catch { }

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
            var semaphore = _gameJoin0Semaphores.GetOrAdd(
                (applicationId, gameId),
                _ => new SemaphoreSlim(1, 1)
            );

            var queue = _gameJoin0RequestQueue.GetOrAdd(
                (applicationId, gameId),
                _ => new ConcurrentQueue<Func<Task>>()
            );

            var enqueueJob = Task.Run(() =>
            {
                queue.Enqueue(taskFunc);
            });

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
                    catch { }

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

            foreach (var appIds in _appIdGroups.Values)
            {
                foreach (var appId in appIds)
                {
                    if (_lookupsByAppId.TryGetValue(appId, out var quickLookup))
                    {
                        lock (quickLookup.AppIdToChannel)
                        {
                            channels.AddRange(
                                quickLookup
                                    .AppIdToChannel.Where(pair => pair.Key == appId)
                                    .SelectMany(pair => pair.Value)
                                    .ToList()
                            );
                        }
                    }
                }
            }

            return channels;
        }

        public List<Channel> GetAllChannels(int appId)
        {
            List<Channel> channels = new();

            if (_lookupsByAppId.TryGetValue(appId, out var quickLookup))
            {
                lock (quickLookup.AppIdToChannel)
                {
                    channels.AddRange(
                        quickLookup
                            .AppIdToChannel.Where(pair => pair.Key == appId)
                            .SelectMany(pair => pair.Value)
                            .ToList()
                    );
                }
            }

            return channels;
        }

        public Channel? GetChannelByChannelId(int channelId, int appId)
        {
            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        var channel = quickLookup
                            .AppIdToChannel.SelectMany(kv => kv.Value)
                            .Where(c => c.Id == channelId && c.ApplicationId == appId)
                            .FirstOrDefault();
                        if (channel != null)
                            return channel;
                    }
                }
            }

            return null;
        }

        public Channel? GetChannelByChannelName(string channelName, int appId)
        {
            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        var channel = quickLookup
                            .AppIdToChannel.SelectMany(kv => kv.Value)
                            .FirstOrDefault(x => x.Name == channelName && x.ApplicationId == appId);
                        if (channel != null)
                            return channel;
                    }
                }
            }

            return null;
        }

        public Channel? GetChannelByRequestFilter(
            int appId,
            ChannelType type,
            ulong FieldMask1,
            ulong FieldMask2,
            ulong FieldMask3,
            ulong FieldMask4,
            MediusLobbyFilterMaskLevelType filterMaskLevelType
        )
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x =>
                    x.Type == type
                    && x.ApplicationId == appId
                    && x.GenericField1 == FieldMask1
                    && x.GenericField2 == FieldMask2
                    && x.GenericField3 == FieldMask3
                    && x.GenericField4 == FieldMask4
                    && x.GenericFieldLevel == (MediusWorldGenericFieldLevelType)filterMaskLevelType
                )
                .First();
        }

        public uint GetChannelCount(ChannelType type, int appId)
        {
            uint count = 0;

            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        count += (uint)
                            quickLookup
                                .AppIdToChannel.SelectMany(kv => kv.Value)
                                .Count(x => x.Type == type && x.ApplicationId == appId);
                    }
                }
            }

            return count;
        }

        public Channel GetOrCreateDefaultLobbyChannel(int appId, int MediusVersion)
        {
            Channel? channel = null;

            foreach (var appIdInGroup in GetAppIdsInGroup(appId))
            {
                if (_lookupsByAppId.TryGetValue(appIdInGroup, out var quickLookup))
                {
                    lock (quickLookup.AppIdToChannel)
                    {
                        channel = quickLookup
                            .AppIdToChannel.SelectMany(kv => kv.Value)
                            .FirstOrDefault(x => x.ApplicationId == appId && x.Id == 1);
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
            if (!_lookupsByAppId.TryGetValue(channel.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(channel.ApplicationId, quickLookup = new QuickLookup());

            lock (quickLookup.AppIdToChannel)
            {
                if (!quickLookup.AppIdToChannel.TryGetValue(channel.ApplicationId, out var value))
                    quickLookup.AppIdToChannel.Add(
                        channel.ApplicationId,
                        new List<Channel>() { channel }
                    );
                else
                    value.Add(channel);
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
        public IEnumerable<Channel> GetChannelListFiltered(
            int appId,
            int pageIndex,
            int pageSize,
            ChannelType type,
            ulong FieldMask1,
            ulong FieldMask2,
            ulong FieldMask3,
            ulong FieldMask4,
            MediusLobbyFilterMaskLevelType filterMaskLevelType
        )
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x =>
                    x.Type == type
                    && x.ApplicationId == appId
                    && x.GenericField1 == FieldMask1
                    && x.GenericField2 == FieldMask2
                    && x.GenericField3 == FieldMask3
                    && x.GenericField4 == FieldMask4
                    && x.GenericFieldLevel == (MediusWorldGenericFieldLevelType)filterMaskLevelType
                )
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
        public IEnumerable<Channel> GetChannelList(
            int appId,
            int pageIndex,
            int pageSize,
            ChannelType type
        )
        {
            return _lookupsByAppId
                .Where(x => GetAppIdsInGroup(appId).Contains(x.Key))
                .SelectMany(x => x.Value.AppIdToChannel.SelectMany(x => x.Value))
                .Where(x => x.Type == type && x.ApplicationId == appId)
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
                .Where(x => x.ApplicationId == appId)
                .OrderBy(kvp => kvp.PlayerCount)
                .First();
        }
        #endregion

        #region Party

        public void JoinParty(ClientObject client, MediusPartyJoinByIndexRequest request)
        {
            List<int> skipGameHostTypeCheckAppIds = new() { }; // Some 108 Medius games not send the same gameHostType in request, but still expect a result...

            List<int> approvedMaxPlayersAppIds = new()
            {
                20624,
                22500,
                22920,
                22924,
                22930,
                23360,
                24000,
                24180,
            };

            var party = GetPartyByPartyId(request.MediusWorldID); // MUM original fetches GameWorldData
            if (party == null)
            {
                LoggerAccessor.LogWarn(
                    $"[MumManager] - Join Game Request Handler Error: Error in retrieving party info from MUM cache [{request.MediusWorldID}]"
                );

                client.Queue(
                    new MediusPartyJoinByIndexResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusNoResult,
                    }
                );
            }
            #region Password
            else if (
                party.PartyPassword != null
                && party.PartyPassword != string.Empty
                && party.PartyPassword != request.PartyPassword
            )
            {
                client.Queue(
                    new MediusPartyJoinByIndexResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusInvalidPassword,
                    }
                );
            }
            #endregion
            #region WorldStatus
            else if (party.WorldStatus == MediusWorldStatus.WorldClosed)
            {
                client.Queue(
                    new MediusJoinGameResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusRequestDenied,
                    }
                );
            }
            #endregion
            #region MaxPlayers
            else if (party.PlayerCount >= party.MaxPlayers)
            {
                client.Queue(
                    new MediusPartyJoinByIndexResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusWorldIsFull,
                    }
                );
            }
            #endregion
            #region GameHostType check
            else if (
                !skipGameHostTypeCheckAppIds.Contains(client.ApplicationId)
                && request.PartyHostType != party.PartyHostType
            )
            {
                client.Queue(
                    new MediusPartyJoinByIndexResponse()
                    {
                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(client.ApplicationId),
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusRequestDenied,
                    }
                );
            }
            #endregion
            else
            {
                _ = EnqueueJoinGameAsync(
                    client.ApplicationId,
                    party.MediusWorldId,
                    () =>
                    {
                        // if This is a Peer to Peer Player Host as DME we treat differently
                        if (party.PartyHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
                        {
                            party.Host?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{party.MediusWorldId}-{client.AccountId}-{request.MessageID}-{1}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP,
                                        TargetWorldID = party.MediusWorldId,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                    },
                                }
                            );
                        }
                        // Else send normal Connection type
                        else
                        {
                            party.DMEServer?.Queue(
                                new MediusServerJoinGameRequest()
                                {
                                    MessageID = new MessageId(
                                        $"{party.MediusWorldId}-{client.AccountId}-{request.MessageID}-{1}"
                                    ),
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        Type = NetConnectionType.NetConnectionTypeClientServerTCP,
                                        TargetWorldID = party.MediusWorldId,
                                        AccessKey = client.AccessToken,
                                        SessionKey = client.SessionKey,
                                        ServerKey = new RSA_KEY(
                                            LIBRARY
                                                .Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey.N.ToByteArrayUnsigned()
                                                .Reverse()
                                                .ToArray()
                                        ),
                                    },
                                }
                            );
                        }

                        return Task.CompletedTask;
                    }
                );
            }
        }

        public async Task AddParty(Party party)
        {
            if (!_lookupsByAppId.TryGetValue(party.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(party.ApplicationId, quickLookup = new QuickLookup());

            quickLookup.PartyIdToGame.Add(party.MediusWorldId, party);
            await HorizonServerConfiguration
                .Database.CreateParty(party.ToPartyDTO())
                .ConfigureAwait(false);
        }

        public async Task CreateParty(ClientObject client, MediusPartyCreateRequest request)
        {
            if (!_lookupsByAppId.TryGetValue(client.ApplicationId, out var quickLookup))
                _lookupsByAppId.TryAdd(client.ApplicationId, quickLookup = new QuickLookup());

            var appIdsInGroup = GetAppIdsInGroup(client.ApplicationId);
            string partyName = request.PartyName;

            var existingParties = _lookupsByAppId
                .Where(x => appIdsInGroup.Contains(client.ApplicationId))
                .SelectMany(x => x.Value.PartyIdToGame.Select(g => g.Value));

            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (
                existingParties.Any(x =>
                    x.WorldStatus != MediusWorldStatus.WorldClosed
                    && x.WorldStatus != MediusWorldStatus.WorldInactive
                    && x.PartyName == partyName
                    && x.Host != null
                    && x.Host.IsConnected
                )
            )
            {
                client.Queue(
                    new RT_MSG_SERVER_APP()
                    {
                        Message = new MediusCreateGameResponse()
                        {
                            MessageID = request.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusGameNameExists,
                        },
                    }
                );
                return;
            }

            // P2P Matchmaking.
            if (request.PartyHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
            {
                // Create and add
                try
                {
                    // Try to get next free MPS server
                    // If none exist, return error to clist
                    var mps = Program.MediusManager?.ProxyServer;
                    if (mps == null)
                    {
                        client.Queue(
                            new MediusMatchCreateGameResponse()
                            {
                                MessageID = request.MessageID,
                                MediusWorldID = -1,
                                StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                            }
                        );
                        return;
                    }

                    var dme = client;

                    Party party = new(client, request, dme);

                    await client.JoinPartyP2P(party).ConfigureAwait(false);

                    await AddParty(party).ConfigureAwait(false);

                    mps.SendServerCreateGameOrPartyWithAttributesRequestP2P(
                        request.MessageID.ToString(),
                        client.AccountId,
                        party.MediusWorldId,
                        party,
                        client
                    );

                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in CreateParty P2P {e}");
                }
            }
            // DME
            else
            {
                // Try to get next free dme server
                // If none exist, return error to clist
                var dme = Program.MediusManager?.ProxyServer.GetFreeDme(
                    client.ApplicationId,
                    client.LocationId
                );

                if (dme == null)
                {
                    client.Queue(
                        new MediusPartyCreateResponse()
                        {
                            MessageID = request.MessageID,
                            MediusWorldID = -1,
                            StatusCode = MediusCallbackStatus.MediusTransactionTimedOut,
                        }
                    );
                    return;
                }

                // Create and add party.
                try
                {
                    Party? party = new(client, request, dme);

                    await AddParty(party).ConfigureAwait(false);

                    // Send create game request to dme server
                    dme.Queue(
                        new MediusServerCreateGameWithAttributesRequest()
                        {
                            MessageID = new MessageId(
                                $"{party.MediusWorldId}-{client.AccountId}-{request.MessageID}-{1}"
                            ),
                            WorldID = party.GameChannel!.Id,
                            Attributes = party.Attributes,
                            ApplicationID = client.ApplicationId,
                            MaxClients = party.MaxPlayers,
                        }
                    );
                    return;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - Error in CreateParty {e}");
                }
            }

            // Failure adding game for some reason
            client.Queue(
                new MediusPartyCreateResponse()
                {
                    MessageID = request.MessageID,
                    MediusWorldID = -1,
                    StatusCode = MediusCallbackStatus.MediusFail,
                }
            );
        }

        #endregion

        #region MFS
        public IEnumerable<MediusFile> GetFilesList(
            string path,
            string filenameBeginsWith,
            uint pageSize,
            uint startingEntryNumber,
            int appId
        )
        {
            lock (_mediusFiles)
            {
                _mediusFiles.Clear();

                if (startingEntryNumber == 0)
                    return _mediusFiles;

                try
                {
                    // Normalize pattern
                    string searchPattern =
                        string.IsNullOrWhiteSpace(filenameBeginsWith) || filenameBeginsWith == "*"
                            ? "*"
                            : filenameBeginsWith;

                    var files = Directory.GetFiles(path, searchPattern).OrderBy(f => f).ToArray();

                    int startIndex = (int)startingEntryNumber - 1;

                    if (startIndex < 0 || startIndex >= files.Length)
                        return _mediusFiles;

                    int endIndex = Math.Min(startIndex + (int)pageSize, files.Length);

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var filePath = files[i];
                        var fi = new FileInfo(filePath);

                        _mediusFiles.Add(
                            new MediusFile
                            {
                                FileName = Path.GetFileName(filePath),
                                FileID = i,
                                FileSize = (int)fi.Length,
                                CreationTimeStamp = (int)fi.CreationTime.ToUnixTimeU32(),
                            }
                        );
                    }
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - MFS FileList Exception: {e}");
                }

                return _mediusFiles;
            }
        }

        public IEnumerable<MediusFile> GetFilesListExt(
            string path,
            string filenameBeginsWith,
            uint pageSize,
            uint startingEntryNumber,
            int appId
        )
        {
            lock (_mediusFiles)
            {
                _mediusFiles.Clear();

                if (startingEntryNumber == 0)
                    return _mediusFiles;

                try
                {
                    string searchPattern = string.IsNullOrWhiteSpace(filenameBeginsWith)
                        ? "*"
                        : filenameBeginsWith;

                    var files = Directory.GetFiles(path, searchPattern).OrderBy(f => f).ToArray();

                    int startIndex = (int)startingEntryNumber - 1;

                    if (startIndex < 0 || startIndex >= files.Length)
                        return _mediusFiles;

                    int endIndex = Math.Min(startIndex + (int)pageSize, files.Length);

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var filePath = files[i];
                        var fi = new FileInfo(filePath);

                        _mediusFiles.Add(
                            new MediusFile
                            {
                                FileName = Path.GetFileName(filePath),
                                FileID = i,
                                FileSize = (int)fi.Length,
                                CreationTimeStamp = (int)fi.CreationTime.ToUnixTimeU32(),
                            }
                        );
                    }
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[MumManager] - MFS FileListExt Exception: {e}");
                }

                return _mediusFiles;
            }
        }

        public static Task UploadMediusFile(
            MediusFileUploadResponse fileUploadResponse,
            ClientObject clientObject
        )
        {
            var uploadState = clientObject.Upload;

            if (fileUploadResponse.iXferStatus >= MediusFileXferStatus.End)
                return Task.CompletedTask;

            try
            {
#if DEBUG
                LoggerAccessor.LogInfo(
                    $"[MumManager] - MFS Bytes Received Total [{uploadState.BytesReceived}] < [{uploadState.TotalSize}]"
                );
#endif
                uploadState.Stream.Seek(fileUploadResponse.iStartByteIndex, SeekOrigin.Begin);
                uploadState.Stream.Write(fileUploadResponse.Data, 0, fileUploadResponse.iDataSize);
                uploadState.BytesReceived += fileUploadResponse.iDataSize;
                uploadState.PacketNumber++;

                if (uploadState.BytesReceived < uploadState.TotalSize)
                {
                    clientObject.Queue(
                        new MediusFileUploadServerRequest()
                        {
                            MessageID = fileUploadResponse.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess,
                            iPacketNumber = uploadState.PacketNumber,
                            iReqStartByteIndex = uploadState.BytesReceived,
                            iXferStatus = MediusFileXferStatus.Mid,
                        }
                    );
                }
                else
                {
                    clientObject.Queue(
                        new MediusFileUploadServerRequest()
                        {
                            MessageID = fileUploadResponse.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess,
                            iPacketNumber = uploadState.PacketNumber,
                            iReqStartByteIndex = uploadState.BytesReceived,
                            iXferStatus = MediusFileXferStatus.End,
                        }
                    );
                }
            }
            catch
            {
                clientObject.Queue(
                    new MediusFileUploadServerRequest()
                    {
                        MessageID = fileUploadResponse.MessageID,
                        StatusCode = MediusCallbackStatus.MediusFileInternalAccessError,
                        iPacketNumber = uploadState.PacketNumber,
                        iReqStartByteIndex = uploadState.BytesReceived,
                        iXferStatus = MediusFileXferStatus.Error,
                    }
                );
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

            return _lookupsByAppId
                .Where(x => appIdsInGroup.Contains(x.Key))
                .SelectMany(x => x.Value.BuddyInvitationsToClient.Select(x => x.Value))
                .ToList();
        }

        public List<AccountDTO> GetBuddyInvitations(int appId, int AccountId)
        {
            var appIdsInGroup = GetAppIdsInGroup(appId);

            return _lookupsByAppId
                .Where(x => appIdsInGroup.Contains(x.Key))
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
            await TickClients().ConfigureAwait(false);

            await TickChannels().ConfigureAwait(false);

            await TickGames().ConfigureAwait(false);
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

                        foreach (var channel in channelKeyPair.Value)
                        {
                            if (channel.ReadyToDestroy)
                                channelsToRemove.Add(channel);
                            else
                                await channel.Tick().ConfigureAwait(false);
                        }

                        lock (quickLookup.Value.AppIdToChannel)
                        {
                            foreach (var channel in channelsToRemove)
                            {
                                LoggerAccessor.LogWarn(
                                    $"[MumManager] - Destroying Channel {channel}"
                                );

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
                LoggerAccessor.LogError($"[MumManager] - Error in TickChannels {ex}");
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
                    var appid = quickLookup.Key;
                    var gameKeysToUpdate = new List<int>();
                    var partyKeysToUpdate = new List<int>();

                    foreach (var gameKeyPair in quickLookup.Value.GameIdToGame)
                    {
                        var game = gameKeyPair.Value;

                        if (game.MediusWorldId != gameKeyPair.Key)
                        {
                            LoggerAccessor.LogWarn(
                                $"[MumManager] - Game Id mismatch: key {gameKeyPair.Key} != game.MediusWorldId {game.MediusWorldId}, updating."
                            );
                            gameKeysToUpdate.Add(gameKeyPair.Key);
                        }
                        if (game.ReadyToDestroy)
                        {
                            LoggerAccessor.LogWarn($"[MumManager] - Destroying Game {game}");
                            await game.EndGame(appid).ConfigureAwait(false);
                            gamesToRemove.Enqueue((quickLookup.Value, gameKeyPair.Key));
                        }
                        else if (game.Destroyed)
                        {
                            LoggerAccessor.LogWarn(
                                $"[MumManager] - Removing destroyed Game {game}"
                            );
                            gamesToRemove.Enqueue((quickLookup.Value, gameKeyPair.Key));
                        }
                        else
                            await game.Tick().ConfigureAwait(false);
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
                            LoggerAccessor.LogWarn(
                                $"[MumManager] - Party Id mismatch: key {partyKeyPair.Key} != party.MediusWorldId {party.MediusWorldId}, updating."
                            );
                            partyKeysToUpdate.Add(partyKeyPair.Key);
                        }
                        if (party.ReadyToDestroy)
                        {
                            LoggerAccessor.LogWarn($"[MumManager] - Destroying Party {party}");
                            await party.EndParty(appid).ConfigureAwait(false);
                            partiesToRemove.Enqueue((quickLookup.Value, partyKeyPair.Key));
                        }
                        else if (party.Destroyed)
                        {
                            LoggerAccessor.LogWarn(
                                $"[MumManager] - Removing destroyed Party {party}"
                            );
                            partiesToRemove.Enqueue((quickLookup.Value, partyKeyPair.Key));
                        }
                        else
                            await party.Tick().ConfigureAwait(false);
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
                LoggerAccessor.LogError($"[MumManager] - Error in TickGames {ex}");
            }
        }

        private async Task TickClients()
        {
            try
            {
                Queue<(int, string)> clientsToRemove = new();

                while (_addQueue.TryDequeue(out var newClient))
                {
                    if (!_lookupsByAppId.TryGetValue(newClient.ApplicationId, out var quickLookup))
                        _lookupsByAppId.TryAdd(
                            newClient.ApplicationId,
                            quickLookup = new QuickLookup()
                        );

                    try
                    {
                        if (newClient.IsLoggedIn)
                        {
                            quickLookup.AccountIdToClient.Add(newClient.AccountId, newClient);

                            if (!string.IsNullOrEmpty(newClient.AccountName))
                                quickLookup.AccountNameToClient.Add(
                                    newClient.AccountName.ToLower(),
                                    newClient
                                );
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
                                    quickLookup.AccountNameToClient.Remove(
                                        newClient.AccountName.ToLower()
                                    );
                            }

                            if (!string.IsNullOrEmpty(newClient.AccessToken))
                                quickLookup.AccessTokenToClient.Remove(newClient.AccessToken);

                            if (!string.IsNullOrEmpty(newClient.SessionKey))
                                quickLookup.SessionKeyToClient.Remove(newClient.SessionKey);
                        }

                        LoggerAccessor.LogError(
                            $"[MumManager] - Error in TickClients addQueue cleanup {e}"
                        );
                    }
                }

                foreach (var quickLookup in _lookupsByAppId)
                {
                    foreach (var clientKeyPair in quickLookup.Value.SessionKeyToClient)
                    {
                        if (!clientKeyPair.Value.IsConnected)
                        {
                            LoggerAccessor.LogWarn(
                                $"[MumManager] - Destroying Client {clientKeyPair.Value}"
                            );

                            // end server session and Logout
                            await clientKeyPair.Value.Logout().ConfigureAwait(false);
                            clientKeyPair.Value.EndServerSession();

                            clientsToRemove.Enqueue((quickLookup.Key, clientKeyPair.Key));
                        }
                        else if (clientKeyPair.Value.Timedout)
                            clientKeyPair.Value.ForceDisconnect();
                    }
                }

                // Remove
                while (clientsToRemove.TryDequeue(out var appIdAndSessionKey))
                {
                    if (_lookupsByAppId.TryGetValue(appIdAndSessionKey.Item1, out var quickLookup))
                    {
                        if (
                            quickLookup.SessionKeyToClient.Remove(
                                appIdAndSessionKey.Item2,
                                out var clientObject
                            )
                        )
                        {
                            if (!string.IsNullOrEmpty(clientObject.AccessToken))
                                quickLookup.AccessTokenToClient.Remove(clientObject.AccessToken);

                            quickLookup.AccountIdToClient.Remove(clientObject.AccountId);

                            if (!string.IsNullOrEmpty(clientObject.AccountName))
                                quickLookup.AccountNameToClient.Remove(
                                    clientObject.AccountName.ToLower()
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MumManager] - Error in TickClients {ex}");
            }
        }

        #endregion

        #region App Ids

        public async Task OnDatabaseAuthenticated()
        {
            // get supported app ids
            var appids = await HorizonServerConfiguration
                .Database.GetAppIds()
                .ConfigureAwait(false);

            if (appids != null)
                // build dictionary of app ids from response
                _appIdGroups = appids.ToDictionary(
                    x => x.Name,
                    x => x.AppIds != null ? x.AppIds.ToArray() : Array.Empty<int>()
                );
        }

        public bool IsAppIdSupported(int appId)
        {
            return _appIdGroups.Any(x => x.Value.Contains(appId));
        }

        public int[] GetAppIdsInGroup(int appId)
        {
            return _appIdGroups.FirstOrDefault(x => x.Value.Contains(appId)).Value
                ?? Array.Empty<int>();
        }

        #endregion

        #region Misc

        #region AnonymouseAccountIdGenerator
        /// <summary>
        /// Generates a Random Anonymous AccountID for MediusAnonymouseAccountRequest <br></br>
        /// Or if one doesn't exist in Database
        /// </summary>
        /// <param name="AnonymousIDRangeSeed">Config Value for changing the MAS</param>
        /// <returns></returns>
        public static int AnonymousAccountIDGenerator(int AnonymousIDRangeSeed)
        {
            // Anonymous login expect a negative id < 0.
            return new Random().Next(-80000000, AnonymousIDRangeSeed);
        }
        #endregion

        #endregion

        #region Matchmaking
        public static int CalculateSizeOfMatchRoster(MediusMatchRosterInfo roster)
        {
            int rosterSize;
            uint v3;
            uint partySize;

            var mediusMatchPartyInfo = new MediusMatchPartyInfo();

            if (roster == null)
                return 0;
            rosterSize = (4 * roster.NumParties) + 8;
            partySize = (uint)roster.Parties;
            v3 = (uint)((4 * roster.NumParties) + partySize - 4);
            while (partySize <= v3)
            {
                rosterSize += CalculateSizeOfMatchParty(mediusMatchPartyInfo);
                partySize += 4;
            }

            return rosterSize;
        }

        public static int CalculateSizeOfMatchParty(MediusMatchPartyInfo party)
        {
            var MatchPartySize = party != null ? (8 * party.NumPlayers) + 8 : 0;
            return MatchPartySize;
        }
        #endregion
    }
}
