using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetNewsRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

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
