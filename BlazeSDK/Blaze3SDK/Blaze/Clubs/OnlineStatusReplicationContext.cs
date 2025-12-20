using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0x27079223)]
	public struct OnlineStatusReplicationContext
	{

		[TdfMember("OLDS")]
		public MemberOnlineStatus mOldMemberOnlineStatus;

		[TdfMember("CLID")]
		public uint mOldSpecificClubId;

		[TdfMember("CURE")]
		public UpdateReason mUpdateReason;

	}
}
