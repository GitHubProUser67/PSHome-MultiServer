using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubsRequest
    {
        
        [TdfMember("CLID")]
        public List<uint> mClubIdList;
        
    }
}
