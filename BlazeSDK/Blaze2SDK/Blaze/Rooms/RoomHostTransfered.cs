using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomHostTransfered
    {
        
        [TdfMember("MBID")]
        public uint mUserId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
