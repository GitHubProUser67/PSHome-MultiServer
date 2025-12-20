using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyPlatformHostInitialized
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PHST")]
		public byte mPlatformHostSlotId;

	}
}
