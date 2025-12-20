using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubBansRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
    }
}
