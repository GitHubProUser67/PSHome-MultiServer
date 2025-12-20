using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct DraftProfile
	{

		[TdfMember("PLRS")]
		public List<Player> mPlayers;

		[TdfMember("RDPF")]
		public List<PositionPref> mRoundPositionPrefs;

		[TdfMember("STRT")]
		public ushort mStartingRank;

	}
}
