using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct GetContentTopNRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("COUN")]
		public int mCount;

		[TdfMember("FLTR")]
		public List<LeaderboardFilter> mFilters;

		[TdfMember("LB")]
		public LeaderboardType mLeaderboardType;

		[TdfMember("VIEW")]
		public string mLeaderboardView;

		[TdfMember("STRT")]
		public int mStart;

		[TdfMember("TAG")]
		public string mTag;

	}
}
