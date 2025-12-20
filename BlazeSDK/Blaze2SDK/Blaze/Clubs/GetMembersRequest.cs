using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetMembersRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
    }
}
