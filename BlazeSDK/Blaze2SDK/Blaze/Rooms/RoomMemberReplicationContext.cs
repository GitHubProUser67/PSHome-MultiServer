using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomMemberReplicationContext
    {
        
        [TdfMember("UPRE")]
        public RoomMemberReplicationReason mReason;
        
        [TdfMember("USID")]
        public uint mUserId;
        
        public enum RoomMemberReplicationReason : int
        {
            USER_LEFT = 0x0,
            USER_KICKED = 0x1,
            USER_JOINED = 0x2,
            USER_LOGOUT = 0x3,
            ROOM_CREATED = 0x4,
            ROOM_DESTROYED = 0x5,
        }
        
    }
}
