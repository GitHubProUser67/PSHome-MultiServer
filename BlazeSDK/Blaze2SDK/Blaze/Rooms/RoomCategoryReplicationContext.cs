using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomCategoryReplicationContext
    {

        [TdfMember("SEID")]
        public uint mUserSessionId;

        [TdfMember("UPRE")]
        public RoomCategoryUpdateReason mUpdateReason;

        [TdfMember("USID")]
        public uint mUserId;

        public enum RoomCategoryUpdateReason : int
        {
            CONFIG_RELOADED = 0x0,
            USER_CREATED = 0x1,
            ROOM_EXPANSION_CHANGE = 0x2,
            ATTRIBUTES_UPDATE = 0x3,
        }

    }
}
