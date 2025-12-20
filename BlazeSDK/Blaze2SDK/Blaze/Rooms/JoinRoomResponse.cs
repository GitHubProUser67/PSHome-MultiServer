using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct JoinRoomResponse
    {
        
        [TdfMember("CDAT")]
        public RoomCategoryData mCategoryData;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        [StringLength(64)]
        public string mFailedCriteria;
        
        [TdfMember("MDAT")]
        public RoomMemberData mMemberData;
        
        [TdfMember("RDAT")]
        public RoomData mRoomData;
        
        [TdfMember("VDAT")]
        public RoomViewData mViewData;
        
        [TdfMember("VERS")]
        public uint mMapVersion;
        
    }
}
