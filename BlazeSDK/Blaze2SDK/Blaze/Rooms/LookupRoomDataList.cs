using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct LookupRoomDataList
    {
        
        [TdfMember("RMSL")]
        public List<RoomData> mRoomDataList;
        
    }
}
