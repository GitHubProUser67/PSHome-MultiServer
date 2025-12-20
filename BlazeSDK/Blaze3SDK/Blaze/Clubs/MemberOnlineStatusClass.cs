using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct MemberOnlineStatusClass
	{

		[TdfMember("MOST")]
		public MemberOnlineStatus mMemberOnlineStatus;

		[TdfMember("MSST")]
		public MembershipStatus mMembershipStatus;

	}
}
