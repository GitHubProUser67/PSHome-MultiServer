using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct ListGameData
	{

		[TdfMember("GAME")]
		public ReplicatedGameData mGame;

		[TdfMember("PROS")]
		public List<ReplicatedGamePlayer> mGameRoster;

	}
}
