using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetInvitationsResponse
	{

		[TdfMember("CIST")]
		public List<ClubMessage> mClubInvList;

	}
}
