using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct BanUnbanMemberRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("UID")]
        public uint mUserId;
        
    }
}
