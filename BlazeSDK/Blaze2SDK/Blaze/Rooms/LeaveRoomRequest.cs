using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct LeaveRoomRequest
    {
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
