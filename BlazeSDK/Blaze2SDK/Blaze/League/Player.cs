using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct Player
    {
        
        [TdfMember("LINE")]
        public uint mLineup;
        
        [TdfMember("PLID")]
        public uint mPlayerId;
        
        [TdfMember("POSI")]
        public uint mPosition;
        
        [TdfMember("RTNG")]
        public uint mRating;
        
        [TdfMember("TEAM")]
        public uint mTeamId;
        
    }
}
