using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateCachedMemberOnlineStatusRequest
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("NMOS")]
		public MemberOnlineStatus mOnlineStatus;

		[TdfMember("UPRS")]
		public UpdateReason mReason;

	}
}
