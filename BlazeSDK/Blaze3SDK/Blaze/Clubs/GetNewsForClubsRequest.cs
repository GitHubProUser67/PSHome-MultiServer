using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetNewsForClubsRequest
	{

		[TdfMember("CIDL")]
		public List<uint> mClubIdList;

		[TdfMember("MCNT")]
		public uint mMaxResultCount;

		[TdfMember("OFST")]
		public uint mOffSet;

		[TdfMember("NSOT")]
		public TimeSortType mSortType;

		[TdfMember("TFIL")]
		public List<NewsType> mTypeFilters;

	}
}
