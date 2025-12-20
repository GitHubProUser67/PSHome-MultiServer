using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct BanUnbanMemberRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
