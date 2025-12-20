using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct SetNewsItemHiddenRequest
    {
        
        [TdfMember("BLID")]
        public ulong mNewsId;
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("ISHD")]
        public bool mIsHidden;
        
    }
}
