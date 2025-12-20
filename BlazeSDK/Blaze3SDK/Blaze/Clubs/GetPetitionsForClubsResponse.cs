using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetPetitionsForClubsResponse
	{

		[TdfMember("LPMP")]
		public SortedDictionary<uint, List<ClubMessage>> mClubPetitionListMap;

	}
}
