using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubMemberships
	{

		[TdfMember("CMSL")]
		public List<ClubMembership> mClubMembershipList;

	}
}
