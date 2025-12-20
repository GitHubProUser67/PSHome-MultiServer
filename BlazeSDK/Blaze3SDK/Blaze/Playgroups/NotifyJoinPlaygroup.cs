using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyJoinPlaygroup
	{

		[TdfMember("USER")]
		public long mJoiningBlazeId;

		[TdfMember("INFO")]
		public PlaygroupInfo mPlaygroupInfo;

		[TdfMember("MLST")]
		public List<PlaygroupMemberInfo> mPlaygroupMemberInfos;

	}
}
