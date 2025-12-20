using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct PlayoffGameScore
    {
        
        [TdfMember("P1SC")]
        public int mPlayer1Score;
        
        [TdfMember("P2SC")]
        public int mPlayer2Score;
        
        [TdfMember("SIMG")]
        public byte mIsSimulatedGame;
        
    }
}
