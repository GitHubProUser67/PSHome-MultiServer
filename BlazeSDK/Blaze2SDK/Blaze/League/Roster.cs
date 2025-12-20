using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct Roster
    {
        
        [TdfMember("CONT")]
        public uint mCount;
        
        [TdfMember("CRC")]
        public uint mCrc;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("MMBR")]
        public LeagueUser mMember;
        
        [TdfMember("PLYR")]
        public List<Player> mPlayers;
        
    }
}
