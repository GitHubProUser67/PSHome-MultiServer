using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct KickUserRequest
    {
        
        [TdfMember("BANU")]
        public bool mBanUser;
        
        [TdfMember("MBID")]
        public uint mUserId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
