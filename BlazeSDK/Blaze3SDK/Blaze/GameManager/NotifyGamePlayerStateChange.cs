using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGamePlayerStateChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("STAT")]
		public PlayerState mPlayerState;

	}
}
