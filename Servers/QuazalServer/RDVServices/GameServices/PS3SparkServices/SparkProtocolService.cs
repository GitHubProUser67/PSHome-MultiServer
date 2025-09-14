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

        private static readonly ConcurrentDictionary<string, List<(GatheringUrls, AnyData<SparkGame>)>> _gameSessions = new ConcurrentDictionary<string, List<(GatheringUrls, AnyData<SparkGame>)>>();

        [RMCMethod(4)]
        public RMCResult CreateGame(AnyData<SparkGame> any_gathering, bool using_net_z, List<StationURL> urls)
        {
            if (Context != null)
            {
                if (any_gathering.data != null)
                {
                    uint gameId = _gameIds.CreateUniqueID();

                    any_gathering.data.gathering.m_idMyself = gameId;
                    any_gathering.data.gathering.m_pidHost = any_gathering.data.gathering.m_pidOwner = Context.Client.PlayerInfo?.PID ?? 0;

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
                            CustomLogger.LoggerAccessor.LogWarn($"[SparkService] - CreateGame - Unknown Spark AccessKey:{accessKey}, unable to update participants count, might break stuff.");
                            break;
                    }

                    if (using_net_z)
                    {
                        StationURL? myUrl = Context.Client.PlayerInfo?.Url;

                        if (myUrl != null)
                            urls.Add(myUrl);
                    }

                    var newSession = (new GatheringUrls { gid = gameId, lst_station_urls = urls }, any_gathering);

                    _gameSessions.AddOrUpdate(
                            accessKey,
                            _ => new List<(GatheringUrls, AnyData<SparkGame>)> { newSession },
                            (_, existingList) =>
                            {
                                existingList.Add(newSession);
                                return existingList;
                            });

                    return Result(new { retVal = gameId });
                }
            }

            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult JoinGame(uint gid)
        {
            if (Context != null && _gameSessions.ContainsKey(Context.Handler.AccessKey))
            {
                var entry = _gameSessions[Context.Handler.AccessKey].Where(game => game.Item1.gid == gid).FirstOrDefault();
                if (entry != default)
                {
                    return Result(new
                    {
                        lst_station_urls = entry.Item1.lst_station_urls,
                        stats = new List<SparkStats> { },
                        spark_game = entry.Item2
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
            if (Context == null)
                return Result(new { retVal = false });

            if (_gameSessions.TryGetValue(Context.Handler.AccessKey, out var sessions) && sessions.RemoveAll(s => s.Item1.gid == gid) > 0)
                return Result(new { retVal = true });

            return Result(new { retVal = false });
        }

        [RMCMethod(16)]
        public RMCResult CancelGame(uint gid, uint reason)
        {
            if (Context == null)
                return Result(new { retVal = false });

            if (_gameSessions.TryGetValue(Context.Handler.AccessKey, out var sessions) && sessions.RemoveAll(s => s.Item1.gid == gid) > 0)
                return Result(new { retVal = true });

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
            // TODO, store the player listing in it's own class alongside the sparkgame

            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(20)]
        public RMCResult BrowseMatchesWithHostUrls()
        {
            if (Context != null && _gameSessions.ContainsKey(Context.Handler.AccessKey))
            {
                var entries = _gameSessions[Context.Handler.AccessKey];
                return Result(new { lst_gathering = entries.Select(s => s.Item2), lst_gathering_urls = entries.Select(s => s.Item1) });
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
    }
}