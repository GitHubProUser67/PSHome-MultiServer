using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models.MatchMakingService;
using QuazalServer.RDVServices.DDL.Models.SparkService;
using System.Collections.Concurrent;

namespace QuazalServer.RDVServices.GameServices.PS3SparkServices
{
    /// <summary>
	/// Secure connection service protocol
	/// </summary>
	[RMCService((ushort)RMCProtocolId.SparkProtocolService)]
    public class SparkProtocolService : RMCServiceBase
    {
        private static readonly UniqueIDGenerator _gameIds = new UniqueIDGenerator();

        private static readonly ConcurrentDictionary<string, ConcurrentList<SparkSession>> _gameSessions = new ConcurrentDictionary<string, ConcurrentList<SparkSession>>();

        [RMCMethod(4)]
        public RMCResult CreateGame(AnyData<SparkGame> any_gathering, bool using_net_z, List<StationURL> urls)
        {
            if (Context != null && Context.Client.PlayerInfo != null && any_gathering.data != null)
            {
                uint gameId = _gameIds.CreateUniqueID();
                uint pid = Context.Client.PlayerInfo.PID;

                any_gathering.data.gathering.m_idMyself = gameId;
                any_gathering.data.gathering.m_pidHost = any_gathering.data.gathering.m_pidOwner = pid;

                string accessKey = Context.Handler.AccessKey;

                switch (accessKey)
                {
                    case "os4R9pEiy":
                        any_gathering.data.gathering.m_uiMinParticipants = 1;
                        any_gathering.data.gathering.m_uiMaxParticipants = 4;
                        break;
                    case "7aK4858Q":
                        any_gathering.data.gathering.m_uiMinParticipants = 1;
                        any_gathering.data.gathering.m_uiMaxParticipants = 8;
                        break;
                    default:
                        CustomLogger.LoggerAccessor.LogError($"[SparkService] - CreateGame - Unknown Spark AccessKey:{accessKey}, unable to update participants count, returning id 0 (error).");
                        return Error(0);
                }

                if (using_net_z)
                {
                    StationURL? myUrl = Context.Client.PlayerInfo.Url;

                    if (myUrl != null)
                        urls.Add(myUrl);
                }

                var newSession = new SparkSession() { URLs = new GatheringUrls { gid = gameId, lst_station_urls = urls }, Game = any_gathering, Participants = new HashSet<uint>() { pid } };

                Context.Client.PlayerInfo.CurrentSparkGameId = gameId;

                _gameSessions.AddOrUpdate(
                        accessKey,
                        _ => new ConcurrentList<SparkSession> { newSession },
                        (_, existingList) =>
                        {
                            existingList.Add(newSession);
                            return existingList;
                        });

                return Result(new { retVal = gameId });
            }

            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult JoinGame(uint gid)
        {
            if (Context != null && Context.Client.PlayerInfo != null && _gameSessions.ContainsKey(Context.Handler.AccessKey))
            {
                var entry = _gameSessions[Context.Handler.AccessKey].Where(game => game.URLs.gid == gid).FirstOrDefault();
                if (entry != default)
                {
                    Context.Client.PlayerInfo.CurrentSparkGameId = entry.Game.data?.gathering.m_idMyself ?? 0;

                    entry.Participants.Add(Context.Client.PlayerInfo.PID);

                    return Result(new
                    {
                        lst_station_urls = entry.URLs.lst_station_urls,
                        stats = new List<SparkStats> { },
                        spark_game = entry.Game
                    });
                }
            }

            return Error(0);
        }

        [RMCMethod(6)]
        public RMCResult GetFriendStats()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(7)]
        public RMCResult GetSelfStats()
        {
            UNIMPLEMENTED();
            return Result(new { retVal = new List<SparkStats> { } });
        }

        [RMCMethod(8)]
        public RMCResult GetParticipationStats()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(9)]
        public RMCResult GetStats()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(10)]
        public RMCResult GetDetailedFriendInfoList()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult GetPlayerStatus()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(12)]
        public RMCResult ReportStats()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(13)]
        public RMCResult GetSecretQuestion()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(14)]
        public RMCResult ValidateSecretAnswer()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(15)]
        public RMCResult EndGame(uint gid, uint reason)
        {
            if (Context == null || Context.Client.PlayerInfo == null)
                return Result(new { retVal = false });

            var accessKey = Context.Handler.AccessKey;

            if (_gameSessions.TryGetValue(accessKey, out var sessions))
            {
                var session = sessions.FirstOrDefault(s => s.URLs.gid == gid);
                if (session != null)
                {
                    sessions.RemoveAll(s => s.URLs.gid == gid);

                    foreach (var pid in session.Participants)
                    {
                        var player = NetworkPlayers.GetPlayerInfoByPID(pid);

                        if (player != null)
                            player.CurrentSparkGameId = 0;
                    }

                    CustomLogger.LoggerAccessor.LogWarn($"[SparkService] - EndGame - Game {gid} ended, all player states reset.");
                    return Result(new { retVal = true });
                }
            }

            return Result(new { retVal = false });
        }

        [RMCMethod(16)]
        public RMCResult CancelGame(uint gid, uint reason)
        {
            if (Context == null || Context.Client.PlayerInfo == null)
                return Result(new { retVal = false });

            var accessKey = Context.Handler.AccessKey;

            if (_gameSessions.TryGetValue(accessKey, out var sessions))
            {
                var session = sessions.FirstOrDefault(s => s.URLs.gid == gid);
                if (session != null)
                {
                    sessions.RemoveAll(s => s.URLs.gid == gid);

                    foreach (var pid in session.Participants)
                    {
                        var player = NetworkPlayers.GetPlayerInfoByPID(pid);

                        if (player != null)
                            player.CurrentSparkGameId = 0;
                    }

                    CustomLogger.LoggerAccessor.LogWarn($"[SparkService] - CancelGame - Game {gid} canceled, all player states reset.");
                    return Result(new { retVal = true });
                }
            }

            return Result(new { retVal = false });
        }


        [RMCMethod(17)]
        public RMCResult GetLeaderboardStats(string LbType)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(18)]
        public RMCResult SelectTheOwnerForPlayAgain()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(19)]
        public RMCResult CloseParticipation()
        {
            return Result(new { retVal = RefreshGames(true, Context?.Client.PlayerInfo) });
        }

        [RMCMethod(20)]
        public RMCResult BrowseMatchesWithHostUrls()
        {
            if (Context != null && _gameSessions.ContainsKey(Context.Handler.AccessKey))
            {
                RefreshGames(false, Context.Client.PlayerInfo);
                var entries = _gameSessions[Context.Handler.AccessKey];
                return Result(new { lst_gathering = entries.Select(s => s.Game), lst_gathering_urls = entries.Select(s => s.URLs) });
            }

            return Error(0); // when no matches found
        }

        [RMCMethod(21)]
        public RMCResult QuickMatchWithHostUrls(bool unk, int unk1, int unk2, int unk3, int unk4)
        {
            UNIMPLEMENTED();
            return Error(0); // when no matches found
        }

        [RMCMethod(22)]
        public RMCResult GetDetailedInvitationsReceivedWithHostUrls()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(23)]
        public RMCResult OpenParticipation()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(25)]
        public RMCResult ReportStatsWithGlobalLeaderboardList()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        public static bool RefreshGames(bool onPlayerRemoval, PlayerInfo? playerToShutdown = null)
        {
            if (!onPlayerRemoval)
            {
                if (playerToShutdown == null)
                {
                    foreach (var kvp in _gameSessions)
                    {
                        var sessions = kvp.Value;

                        foreach (var session in sessions
                            .Where(s => s.Participants.Count < (s.Game.data?.gathering?.m_uiMinParticipants))
                            .ToList())
                        {
                            sessions.Remove(session);

                            foreach (var pid in session.Participants)
                            {
                                var player = NetworkPlayers.GetPlayerInfoByPID(pid);

                                if (player != null)
                                    player.CurrentSparkGameId = 0;
                            }

                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[SparkService] - AutoRemoved game {session.URLs.gid} (below min participants), all player states reset.");
                        }
                    }
                }
                else // Optimize the lookup
                {
                    foreach (var kvp in _gameSessions.Where(g => g.Key == playerToShutdown.AccessKey))
                    {
                        var sessions = kvp.Value;

                        foreach (var session in sessions
                            .Where(s => s.Participants.Count < (s.Game.data?.gathering?.m_uiMinParticipants))
                            .ToList())
                        {
                            sessions.Remove(session);

                            foreach (var pid in session.Participants)
                            {
                                var player = NetworkPlayers.GetPlayerInfoByPID(pid);

                                if (player != null)
                                    player.CurrentSparkGameId = 0;
                            }

                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[SparkService] - AutoRemoved game {session.URLs.gid} (below min participants), all player states reset.");
                        }
                    }
                }

                return true;
            }
            else if (playerToShutdown != null && _gameSessions.ContainsKey(playerToShutdown.AccessKey))
            {
                // Id checked earlier before calling the func.
                var session = _gameSessions[playerToShutdown.AccessKey].Where(game => game.URLs.gid == playerToShutdown.CurrentSparkGameId).FirstOrDefault();
                if (session != default)
                {
                    playerToShutdown.CurrentSparkGameId = 0;

                    session.Participants.Remove(playerToShutdown.PID);

                    if (session.Participants.Count < session.Game.data?.gathering.m_uiMinParticipants && _gameSessions.TryGetValue(playerToShutdown.AccessKey, out var sessions))
                    {
                        sessions.RemoveAll(s => s.URLs.gid == session.URLs.gid);

                        foreach (var pid in session.Participants)
                        {
                            var player = NetworkPlayers.GetPlayerInfoByPID(pid);

                            if (player != null)
                                player.CurrentSparkGameId = 0;
                        }

                        CustomLogger.LoggerAccessor.LogWarn(
                            $"[SparkService] - AutoRemoved game {session.URLs.gid} (below min participants), all player states reset.");
                    }

                    return true;
                }
            }

            return false;
        }
    }
}