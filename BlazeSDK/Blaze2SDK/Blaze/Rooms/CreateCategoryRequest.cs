using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct CreateCategoryRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mRoomAttributes;
        
        [TdfMember("CAPA")]
        public uint mCapacity;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("CMET")]
        public SortedDictionary<string, string> mClientMetaData;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(32)]
        public string mCatName;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteria;
        
        /// <summary>
        /// Max String Length: 384
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(384)]
        public string mDescription;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("GMET")]
        public SortedDictionary<string, string> mGameMetaData;
        
        [TdfMember("JOIN")]
        public bool mJoinIfExists;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(16)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("VNAM")]
        [StringLength(32)]
        public string mViewName;
        
        [TdfMember("VWID")]
        public uint mViewId;
        
    }
}
