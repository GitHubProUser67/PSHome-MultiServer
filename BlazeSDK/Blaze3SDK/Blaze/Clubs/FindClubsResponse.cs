using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct FindClubsResponse
	{

		[TdfMember("CLST")]
		public List<Club> mClubList;

		[TdfMember("CTCT")]
		public uint mTotalCount;

	}
}
