using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0x28DCBF77)]
	public struct ReplicatedCachedMemberOnlineStatus
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("CMOS")]
		public MemberOnlineStatus mMemberOnlineStatus;

	}
}
