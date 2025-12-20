using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetMembersResponse
	{

		[TdfMember("CMLS")]
		public List<ClubMember> mClubMemberList;

		[TdfMember("TCON")]
		public uint mTotalCount;

	}
}
