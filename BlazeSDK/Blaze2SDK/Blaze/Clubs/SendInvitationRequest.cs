using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct SendInvitationRequest
    {
        
        [TdfMember("BLID")]
        public uint mUserId;
        
        [TdfMember("CLID")]
        public uint mClubId;
        
    }
}
