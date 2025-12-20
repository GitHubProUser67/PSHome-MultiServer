using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct KickUserMasterRequest
    {
        
        [TdfMember("BANU")]
        public bool mBanUser;
        
        [TdfMember("MBID")]
        public uint mUserId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
        [TdfMember("SSID")]
        public uint mSessionId;
        
    }
}
