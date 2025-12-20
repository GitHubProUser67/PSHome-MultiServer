using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGamePlayerQueuePositionChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("QID")]
		public byte mQueueIndex;

	}
}
