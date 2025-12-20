using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomViewReplicationContext
    {

        [TdfMember("UPRE")]
        public RoomViewUpdateReason mUpdateReason;

        public enum RoomViewUpdateReason : int
        {
            CONFIG_RELOADED = 0x0,
            USER_ROOM_CREATED = 0x1,
            USER_ROOM_DESTROYED = 0x2,
        }

    }
}
