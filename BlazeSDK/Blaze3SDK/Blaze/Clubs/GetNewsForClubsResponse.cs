using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetNewsForClubsResponse
	{

		[TdfMember("NLMP")]
		public SortedDictionary<uint, List<ClubLocalizedNews>> mLocalizedNewsListMap;

		[TdfMember("TLPG")]
		public ushort mTotalPages;

	}
}
