using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomViewData
    {
        
        /// <summary>
        /// Max String Length: 192
        /// </summary>
        [TdfMember("DISP")]
        [StringLength(192)]
        public string mDisplayName;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("GMET")]
        public SortedDictionary<string, string> mGameMetaData;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("META")]
        public SortedDictionary<string, string> mClientMetaData;
        
        [TdfMember("MXRM")]
        public uint mMaxUserRooms;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("USRM")]
        public uint mNumUserRooms;
        
        [TdfMember("VWID")]
        public uint mViewId;
        
    }
}
