using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct MatchmakingDedicatedServerOverrideRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
