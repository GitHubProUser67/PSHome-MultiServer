using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct PostNewsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("NWLI")]
        public ClubNews mClubNews;
        
    }
}
