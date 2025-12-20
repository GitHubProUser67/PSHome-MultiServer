using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct LeavePlaygroupRequest
	{

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
