using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetMemberCountsResponse
    {
        
        [TdfMember("CONT")]
        public uint mTotalMemberCount;
        
        [TdfMember("OCON")]
        public uint mOnlineMemberCount;
        
        [TdfMember("TNID")]
        public uint mTournamentId;
        
    }
}
