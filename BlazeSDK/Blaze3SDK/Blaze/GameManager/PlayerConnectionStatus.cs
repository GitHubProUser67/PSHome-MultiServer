using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct PlayerConnectionStatus
	{

		[TdfMember("FLGS")]
		public PlayerNetConnectionFlags mPlayerNetConnectionFlags;

		[TdfMember("STAT")]
		public PlayerNetConnectionStatus mPlayerNetConnectionStatus;

		[TdfMember("PID")]
		public long mTargetPlayer;

	}
}
