using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetPetitionsForClubsRequest
	{

		[TdfMember("CIDL")]
		public List<uint> mClubIdList;

		[TdfMember("INVT")]
		public PetitionsType mPetitionsType;

		[TdfMember("NSOT")]
		public TimeSortType mSortType;

	}
}
