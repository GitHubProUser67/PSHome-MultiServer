using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct(0x21239231)]
	public struct GameManagerCensusData
	{

		[TdfMember("GACD")]
		public List<GameAttributeCensusData> mGameAttributesData;

		[TdfMember("AGN")]
		public uint mNumOfActiveGame;

		[TdfMember("JPN")]
		public uint mNumOfJoinedPlayer;

		[TdfMember("LSN")]
		public uint mNumOfLoggedSession;

		[TdfMember("MMSN")]
		public uint mNumOfMatchmakingSession;

	}
}
