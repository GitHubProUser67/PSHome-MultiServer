using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubGameSummary
    {
        
        [TdfMember("NLOS")]
        public uint mLoss;
        
        [TdfMember("NLUP")]
        public uint mLastUpdateTime;
        
        [TdfMember("NTIE")]
        public uint mTie;
        
        [TdfMember("NWIN")]
        public uint mWin;
        
        [TdfMember("OPID")]
        public uint mOppoClubId;
        
    }
}
