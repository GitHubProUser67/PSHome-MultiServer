using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyMemberPermissionsChange
	{

		[TdfMember("LID")]
		public long mBlazeId;

		[TdfMember("PERM")]
		public MemberPermissions mPermissions;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
