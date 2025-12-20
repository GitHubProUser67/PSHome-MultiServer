using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct PromoteToGMRequest
    {
        
        [TdfMember("BLID")]
        public uint mUserId;
        
        [TdfMember("CLID")]
        public uint mClubId;
        
    }
}
