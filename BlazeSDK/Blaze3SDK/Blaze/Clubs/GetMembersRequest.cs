using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetMembersRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("FILM")]
		public MemberTypeFilter mMemberType;

		[TdfMember("OFRC")]
		public uint mOffset;

		[TdfMember("ORDM")]
		public OrderMode mOrderMode;

		[TdfMember("ORDT")]
		public MemberOrder mOrderType;

	}
}
