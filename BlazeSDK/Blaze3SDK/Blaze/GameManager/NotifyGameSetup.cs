using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameSetup
	{

		[TdfMember("GAME")]
		public ReplicatedGameData mGameData;

		[TdfMember("QUEU")]
		public List<ReplicatedGamePlayer> mGameQueue;

		[TdfMember("PROS")]
		public List<ReplicatedGamePlayer> mGameRoster;

		[TdfMember("REAS")]
		public GameSetupReason mGameSetupReason;

	}
}
