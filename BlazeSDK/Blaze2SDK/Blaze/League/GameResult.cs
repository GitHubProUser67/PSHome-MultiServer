using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GameResult
    {
        
        [TdfMember("GMID")]
        public uint mGameId;
        
        [TdfMember("PLRS")]
        public List<LeagueUser> mParticipants;
        
        [TdfMember("SCRS")]
        public List<uint> mScores;
        
        [TdfMember("SIM")]
        public byte mIsSimulated;
        
        [TdfMember("SIZE")]
        public uint mSize;
        
        [TdfMember("TIME")]
        public uint mTime;
        
        [TdfMember("WLTS")]
        public uint mWinner;
        
    }
}
