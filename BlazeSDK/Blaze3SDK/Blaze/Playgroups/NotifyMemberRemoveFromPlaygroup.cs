using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyMemberRemoveFromPlaygroup
	{

		[TdfMember("MLST")]
		public long mBlazeId;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

		[TdfMember("REAS")]
		public PlaygroupMemberRemoveReason mReason;

	}
}
