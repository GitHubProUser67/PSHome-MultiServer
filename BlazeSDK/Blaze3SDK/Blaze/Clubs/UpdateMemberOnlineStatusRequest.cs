using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateMemberOnlineStatusRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("STAT")]
		public MemberOnlineStatus mNewStatus;

	}
}
