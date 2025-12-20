using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameStateChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GSTA")]
		public GameState mNewGameState;

	}
}
