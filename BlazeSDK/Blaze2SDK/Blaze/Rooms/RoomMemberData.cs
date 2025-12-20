using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomMemberData
    {
        
        [TdfMember("BZID")]
        public uint mBlazeId;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
