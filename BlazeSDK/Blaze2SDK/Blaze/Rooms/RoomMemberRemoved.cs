using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomMemberRemoved
    {
        
        [TdfMember("MBID")]
        public uint mUserId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
