using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct AddQueuedPlayerToGameRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
