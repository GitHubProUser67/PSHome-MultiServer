using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct KickPlaygroupMemberRequest
	{

		[TdfMember("EID")]
		public long mBlazeId;

		[TdfMember("REAS")]
		public PlaygroupMemberRemoveReason mKickedReason;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
