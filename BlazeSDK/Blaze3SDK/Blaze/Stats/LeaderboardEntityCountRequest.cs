using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardEntityCountRequest
	{

		[TdfMember("LBID")]
		public int mBoardId;

		[TdfMember("NAME")]
		public string mBoardName;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("POFF")]
		public int mPeriodOffset;

	}
}
