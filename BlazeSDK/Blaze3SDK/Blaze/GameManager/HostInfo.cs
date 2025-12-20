using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct HostInfo
	{

		[TdfMember("HPID")]
		public long mPlayerId;

		[TdfMember("HSLT")]
		public byte mSlotId;

	}
}
