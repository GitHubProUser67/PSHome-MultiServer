using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubBansResponse
    {
        
        [TdfMember("BANS")]
        public SortedDictionary<uint, uint> mUserIdToBanStatusMap;
        
    }
}
