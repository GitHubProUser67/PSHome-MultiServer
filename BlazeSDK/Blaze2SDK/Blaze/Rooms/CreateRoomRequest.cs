using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct CreateRoomRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mRoomAttributes;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteria;
        
        [TdfMember("CTID")]
        public uint mCategoryId;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(16)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 192
        /// </summary>
        [TdfMember("RNAM")]
        [StringLength(192)]
        public string mName;
        
    }
}
