using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomData
    {
        
        [TdfMember("AREM")]
        public bool mAutoRemove;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mAttributes;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(256)]
        public string mCreatorPersonaName;
        
        [TdfMember("CRET")]
        public uint mCreatorUserId;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteria;
        
        [TdfMember("CRTM")]
        public uint mCreationTime;
        
        [TdfMember("CTID")]
        public uint mCategoryId;
        
        [TdfMember("ENUM")]
        public uint mRoomNumber;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("HNAM")]
        [StringLength(256)]
        public string mHostPersonaName;
        
        [TdfMember("HOST")]
        public uint mHostUserId;
        
        /// <summary>
        /// Max String Length: 192
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(192)]
        public string mName;
        
        [TdfMember("POPU")]
        public uint mPopulation;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PSWD")]
        [StringLength(16)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("PVAL")]
        [StringLength(32)]
        public string mPseudoValue;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
        [TdfMember("UCRT")]
        public bool mIsUserCreated;
        
    }
}
