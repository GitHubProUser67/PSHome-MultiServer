using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyMemberJoinedPlaygroup
	{

		[TdfMember("PGID")]
		public uint mPlaygroupId;

		[TdfMember("MEMB")]
		public PlaygroupMemberInfo mPlaygroupMemberInfo;

	}
}
