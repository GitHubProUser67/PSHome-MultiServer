using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameManagerCensusData
    {
        
        [TdfMember("AGN")]
        public uint mNumOfActiveGame;
        
        [TdfMember("GACD")]
        public List<GameAttributeCensusData> mGameAttributesData;
        
        [TdfMember("JPN")]
        public uint mNumOfJoinedPlayer;
        
        [TdfMember("LSN")]
        public uint mNumOfLoggedSession;
        
        [TdfMember("MMSN")]
        public uint mNumOfMatchmakingSession;
        
    }
}
