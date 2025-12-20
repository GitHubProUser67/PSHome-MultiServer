using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubMembershipForUsersResponse
	{

		[TdfMember("MMAP")]
		public SortedDictionary<long, ClubMemberships> mMembershipMap;

	}
}
