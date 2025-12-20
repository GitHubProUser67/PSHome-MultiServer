using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;

namespace QuazalServer.RDVServices
{
	public class DSFPlayerGameData : IPlayerDataStore
	{
		public DSFPlayerGameData(PlayerInfo owner)
		{
			Owner = owner;

			CurrentGatheringId = uint.MaxValue;
            CurrentSession = null;
        }

        public readonly PlayerInfo Owner;

        public uint CurrentGatheringId { get; set; }
        public GameSessionKey CurrentSession { get; set; }
        public PresenceElement CurrentPresence { get; set; }

        public void OnDropped()
		{
            // when player dropped, game data will be destroyed
            // so we need to remove player from game session and gatherings

            // BUG: there is a bug in dropping sessions
            PartySessions.UpdateGatheringParticipation(Owner, uint.MaxValue);
            GameSessions.UpdateSessionParticipation(Owner, null, false);
        }
	}

	public static class DSFPlayerInfoExtensions
	{
		public static DSFPlayerGameData GameData(this PlayerInfo plInfo)
		{
			return plInfo.GetData<DSFPlayerGameData>();
		}
	}
}
