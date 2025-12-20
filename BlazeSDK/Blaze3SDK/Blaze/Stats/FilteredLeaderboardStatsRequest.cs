using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct FilteredLeaderboardStatsRequest
	{

		[TdfMember("LBID")]
		public int mBoardId;

		[TdfMember("NAME")]
		public string mBoardName;

		[TdfMember("FILT")]
		public bool mIncludeStatlessEntities;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("IDLS")]
		public List<long> mListOfIds;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("TIME")]
		public int mTime;

		[TdfMember("USET")]
		public BlazeObjectId mUserSetId;

	}
}
