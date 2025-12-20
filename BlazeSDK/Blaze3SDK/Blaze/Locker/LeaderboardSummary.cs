using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardSummary
	{

		[TdfMember("VIEW")]
		public string mLeaderboardView;

		[TdfMember("SIZE")]
		public int mSize;

		[TdfMember("TAGS")]
		public int mTagsIncluded;

	}
}
