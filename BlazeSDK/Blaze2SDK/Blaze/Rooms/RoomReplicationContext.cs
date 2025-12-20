using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomReplicationContext
    {
        
        [TdfMember("SEID")]
        public uint mUserSessionId;
        
        [TdfMember("UPRE")]
        public RoomUpdateReason mUpdateReason;
        
        [TdfMember("USID")]
        public uint mUserId;
        
        [TdfMember("VWID")]
        public uint mViewId;
        
        public enum RoomUpdateReason : int
        {
            DELETED_EXPANDED_ROOM = 0x0,
            DELETED_PSEUDO_ROOM = 0x1,
            DELETED_EMPTY_USER_CREATED = 0x2,
            DELETED_BY_USER = 0x3,
            CONFIG_RELOADED = 0x4,
            HOST_TRANSFER = 0x5,
            POPULATION_CHANGED = 0x6,
            USER_CREATED = 0x7,
            ATTRIBUTES_UPDATE = 0x8,
        }
        
    }
}
