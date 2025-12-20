using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct MemberStats
	{

		[TdfMember("GOLA")]
		public int mGoalsAgainst;

		[TdfMember("GOLF")]
		public int mGoalsFor;

		[TdfMember("LOSS")]
		public int mLosses;

		[TdfMember("NAME")]
		public string mPersona;

		[TdfMember("PNTS")]
		public int mPoints;

		[TdfMember("RANK")]
		public int mRank;

		[TdfMember("TIES")]
		public int mTies;

		[TdfMember("WINS")]
		public int mWins;

	}
}
