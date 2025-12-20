using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubsRequest
	{

		[TdfMember("CLID")]
		public List<uint> mClubIdList;

		[TdfMember("CODR")]
		public ClubsOrder mClubsOrder;

		[TdfMember("CLTI")]
		public bool mIncludeClubTags;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("OFRC")]
		public uint mOffset;

		[TdfMember("ODMD")]
		public OrderMode mOrderMode;

	}
}
