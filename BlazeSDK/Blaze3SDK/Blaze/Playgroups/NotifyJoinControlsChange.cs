using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyJoinControlsChange
	{

		[TdfMember("PGID")]
		public uint mPlaygroupId;

		[TdfMember("OPEN")]
		public PlaygroupJoinability mPlaygroupJoinability;

	}
}
