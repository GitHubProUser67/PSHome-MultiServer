using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct CreateRoomResponse
    {
        
        [TdfMember("CDAT")]
        public RoomCategoryData mCategoryData;
        
        [TdfMember("MDAT")]
        public RoomMemberData mMemberData;
        
        [TdfMember("RDAT")]
        public RoomData mRoomData;
        
        [TdfMember("VDAT")]
        public RoomViewData mViewData;
        
    }
}
