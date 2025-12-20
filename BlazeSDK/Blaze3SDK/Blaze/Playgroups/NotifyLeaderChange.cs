using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyLeaderChange
	{

		[TdfMember("HSID")]
		public byte mNewHostSlotId;

		[TdfMember("LID")]
		public long mNewLeaderId;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
