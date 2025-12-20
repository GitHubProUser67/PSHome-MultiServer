using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardStatsRequest
	{

		[TdfMember("LBID")]
		public int mBoardId;

		[TdfMember("NAME")]
		public string mBoardName;

		[TdfMember("COUN")]
		public int mCount;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("STRT")]
		public int mRankStart;

		[TdfMember("TIME")]
		public int mTime;

		[TdfMember("USET")]
		public BlazeObjectId mUserSetId;

	}
}
