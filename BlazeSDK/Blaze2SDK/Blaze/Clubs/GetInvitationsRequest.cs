using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetInvitationsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("INVT")]
        public InvitationsType mInvitationsType;
        
        [TdfMember("NSOT")]
        public TimeSortType mSortType;
        
    }
}
