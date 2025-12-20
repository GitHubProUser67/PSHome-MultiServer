using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GameResult
	{

		[TdfMember("GMID")]
		public uint mGameId;

		[TdfMember("SIM")]
		public byte mIsSimulated;

		[TdfMember("PLRS")]
		public List<LeagueUser> mParticipants;

		[TdfMember("SCRS")]
		public List<uint> mScores;

		[TdfMember("SIZE")]
		public uint mSize;

		[TdfMember("TIME")]
		public uint mTime;

		[TdfMember("WLTS")]
		public uint mWinner;

	}
}
