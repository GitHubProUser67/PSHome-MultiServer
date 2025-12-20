using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetUserBansResponse
    {
        
        [TdfMember("BANS")]
        public SortedDictionary<uint, uint> mClubIdToBanStatusMap;
        
    }
}
