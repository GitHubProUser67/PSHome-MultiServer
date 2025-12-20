using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomCategoryData
    {
        
        [TdfMember("CAPA")]
        public uint mCapacity;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("CMET")]
        public SortedDictionary<string, string> mClientMetaData;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteria;
        
        [TdfMember("CTID")]
        public uint mCategoryId;
        
        /// <summary>
        /// Max String Length: 384
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(384)]
        public string mDescription;
        
        /// <summary>
        /// Max String Length: 192
        /// </summary>
        [TdfMember("DISP")]
        [StringLength(192)]
        public string mDisplayName;
        
        /// <summary>
        /// Max String Length: 192
        /// </summary>
        [TdfMember("DISR")]
        [StringLength(192)]
        public string mRoomDisplayName;
        
        [TdfMember("EMAX")]
        public uint mMaxExpandedRooms;
        
        [TdfMember("EPCT")]
        public ushort mExpandThresholdPercent;
        
        [TdfMember("FLAG")]
        public RoomCategoryFlags mFlags;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("GMET")]
        public SortedDictionary<string, string> mGameMetaData;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("LOCL")]
        [StringLength(32)]
        public string mLocale;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("NEXP")]
        public ushort mNumExpandedRooms;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(16)]
        public string mPassword;
        
        [TdfMember("UCRT")]
        public bool mIsUserCreated;
        
        [TdfMember("VWID")]
        public uint mViewId;
        
    }
}
