using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubMembershipForUsersRequest
	{

		[TdfMember("IDLT")]
		public List<long> mBlazeIdList;

	}
}
