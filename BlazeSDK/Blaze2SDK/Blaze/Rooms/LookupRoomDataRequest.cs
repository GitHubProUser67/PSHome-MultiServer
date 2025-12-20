using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct LookupRoomDataRequest
    {
        
        [TdfMember("RMID")]
        public List<uint> mRoomIdList;
        
    }
}
