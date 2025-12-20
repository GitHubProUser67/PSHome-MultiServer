using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardListResponse
	{

		[TdfMember("LIST")]
		public List<LeaderboardSummary> mLeaderboardSummaries;

	}
}
