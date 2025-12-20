using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct MasterMessage
	{

		[TdfMember("SMSG")]
		public SlaveMessage mSlaveMessage;

		[TdfMember("TIME")]
		public List<uint> mTargetSlaveIds;

	}
}
