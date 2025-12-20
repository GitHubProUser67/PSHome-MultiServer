using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubAwardsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
    }
}
