using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct CenteredLeaderboardStatsRequest
	{

		[TdfMember("LBID")]
		public int mBoardId;

		[TdfMember("NAME")]
		public string mBoardName;

		[TdfMember("CENT")]
		public long mCenter;

		[TdfMember("COUN")]
		public int mCount;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("BOTT")]
		public bool mShowAtBottomIfNotFound;

		[TdfMember("TIME")]
		public int mTime;

		[TdfMember("USET")]
		public BlazeObjectId mUserSetId;

	}
}
