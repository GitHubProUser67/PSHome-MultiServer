using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct MemberInfo
    {
        
        [TdfMember("GREM")]
        public byte mGamesRemaining;
        
        [TdfMember("ISDP")]
        public byte mIsDraftProfileSubmitted;
        
        [TdfMember("ISGM")]
        public byte mIsGM;
        
        [TdfMember("ISON")]
        public byte mIsOnline;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("META")]
        public byte[] mMetadata;
        
        [TdfMember("MMBR")]
        public LeagueUser mMember;
        
        [TdfMember("SMET")]
        public byte mIsStringMetadata;
        
        [TdfMember("STAT")]
        public MemberStats mStats;
        
        [TdfMember("TEAM")]
        public uint mTeamId;
        
    }
}
