using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices
{
	public static class PartySessions
	{
		public static List<PartySessionGathering> GatheringList = new();

        /// <summary>
        /// Removes player from all gatherings (except new one) and adds him to new one
        /// </summary>
        /// <param name="playerInfo"></param>
        /// <param name="newGatheringId"></param>
        public static void UpdateGatheringParticipation(PlayerInfo playerInfo, uint newGatheringId)
        {
            // remove this participant from all gatherings
            // except new one and remove station urls associated
            var oldGatherings = GatheringList.Where(x => x.Participants.Contains(playerInfo.PID) && x.Session.m_idMyself != newGatheringId).ToArray();
            foreach (var gathering in oldGatherings)
            {
                gathering.Urls?.RemoveAll(x => x.Compare(playerInfo.Url));
                gathering.Participants?.Remove(playerInfo.PID);
            }

            // add player to new gathering if he isn't there yet
            var newGathering = GatheringList.SingleOrDefault(x => x.Session.m_idMyself == newGatheringId);
            if (newGathering != null && !newGathering.Participants.Contains(playerInfo.PID))
                newGathering.Participants.Add(playerInfo.PID);

            // delete all outdated empty gatherings
            GatheringList.RemoveAll(gathering => {
                if (gathering.Participants.Count > 0)
                    return false;

                CustomLogger.LoggerAccessor.LogWarn($"[PartySessionGathering] - Auto-deleted gathering {gathering.Session.m_idMyself}");
                return true;
            });

            playerInfo.GameData().CurrentGatheringId = newGatheringId;
        }
    }

	public class PartySessionGathering
	{
		public PartySessionGathering()
		{
			Session = new HermesPartySession();
		}

		public PartySessionGathering(HermesPartySession session)
		{
			Session = session;
			Urls = new List<StationURL>();
			Participants = new HashSet<uint>();
		}

		public HermesPartySession Session { get; set; }
		public List<StationURL>? Urls { get; set; } // host and player URLs
		public HashSet<uint>? Participants { get; set; }
	}
}
