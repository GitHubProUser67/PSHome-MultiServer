using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.DDL;
using CustomLogger;

namespace QuazalServer.RDVServices
{
	public static class GameSessions
	{
		public static readonly List<GameSessionData> SessionList = new();

		// NOTE: returns true when session is no longer valid
		public static bool RemovePlayerFromSession(GameSessionData session, uint principalId)
		{
			session.PublicParticipants.Remove(principalId);
			session.Participants.Remove(principalId);

			session.Attributes[(uint)GameSessionAttributeType.FilledPublicSlots] = (uint)session.PublicParticipants.Count;
			session.Attributes[(uint)GameSessionAttributeType.FilledPrivateSlots] = (uint)session.Participants.Count;

            return (session.TotalParticipantCount == 0);
        }

		public static void UpdateSessionParticipation(PlayerInfo playerInfo, GameSessionKey? newSessionKey, bool privateSlot)
		{
            // delete player from old sessions
            var participatingSessions = SessionList.Where(x =>
                x.Participants.Contains(playerInfo.PID) &&
                x.PublicParticipants.Contains(playerInfo.PID) &&
                newSessionKey != null ? !x.IsMatchingKey(newSessionKey) : true
            );

            foreach (var session in participatingSessions)
            {
                session.HostURLs.RemoveAll(x => x.Compare(playerInfo.Url));
                RemovePlayerFromSession(session, playerInfo.PID);
            }

            if (newSessionKey != null)
            {
                var newSession = SessionList.FirstOrDefault(x => x.IsMatchingKey(newSessionKey));
                if (newSession != null)
                {
                    if (privateSlot)
                        newSession.Participants.Add(playerInfo.PID);
                    else
                        newSession.PublicParticipants.Add(playerInfo.PID);

                    newSession.Attributes[(uint)GameSessionAttributeType.FilledPublicSlots] = (uint)newSession.PublicParticipants.Count;
                    newSession.Attributes[(uint)GameSessionAttributeType.FilledPrivateSlots] = (uint)newSession.Participants.Count;
                }
            }

            // delete all outdated empty sessions
            SessionList.RemoveAll(gathering => {
                if (gathering.TotalParticipantCount > 0)
                    return false;

                LoggerAccessor.LogInfo($"[GameSessionData] - UpdateSessionParticipation: Auto-deleted session {gathering.Id}");
                return true;
            });

            playerInfo.GameData().CurrentSession = newSessionKey;
        }
	}

	public class GameSessionData
	{
		public GameSessionData()
		{
			Attributes = new Dictionary<uint, uint>();
			HostURLs = new List<StationURL>();
			Participants = new HashSet<uint>();
			PublicParticipants = new HashSet<uint>();
		}

        public bool IsMatchingKey(GameSessionKey other)
        {
            return Id.Equals(other.m_sessionID) && TypeID.Equals(other.m_typeID);
        }

        public uint Id { get; set; }
        public uint TypeID { get; set; }

		public Dictionary<uint, uint> Attributes { get; set; }
		public uint HostPID { get; set; }
		public List<StationURL> HostURLs { get; set; }
		public HashSet<uint> Participants { get; set; } // ID, Private
		public HashSet<uint> PublicParticipants { get; set; } // ID, Public

        public int TotalParticipantCount { get => Participants.Count + PublicParticipants.Count; }
        public uint[] AllParticipants { get => Participants.Concat(PublicParticipants).ToArray(); }
    }
}
