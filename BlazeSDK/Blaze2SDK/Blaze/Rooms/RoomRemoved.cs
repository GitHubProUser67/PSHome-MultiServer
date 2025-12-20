using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomRemoved
    {
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
