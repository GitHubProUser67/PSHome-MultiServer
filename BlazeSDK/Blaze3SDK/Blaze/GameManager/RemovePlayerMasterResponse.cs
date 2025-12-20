using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RemovePlayerMasterResponse
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GPLY")]
		public ReplicatedGamePlayer mPlayer;

	}
}
