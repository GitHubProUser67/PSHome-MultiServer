using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardListRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("LB")]
		public LeaderboardType mLeaderboardType;

	}
}
