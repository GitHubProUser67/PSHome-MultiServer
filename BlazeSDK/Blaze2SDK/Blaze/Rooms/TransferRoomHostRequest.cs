using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct TransferRoomHostRequest
    {
        
        [TdfMember("BZID")]
        public uint mUserId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
