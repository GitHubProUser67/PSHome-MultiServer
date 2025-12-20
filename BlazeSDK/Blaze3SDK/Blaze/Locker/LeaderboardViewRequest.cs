using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardViewRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("LB")]
		public LeaderboardType mLeaderboardType;

		[TdfMember("VIEW")]
		public string mLeaderboardView;

	}
}
