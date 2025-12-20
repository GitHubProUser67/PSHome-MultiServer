using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct LeaderboardViewColumn
    {
        
        [TdfMember("DISP")]
        public int mDisplay;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("FRMT")]
        [StringLength(32)]
        public string mFormat;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("LDSC")]
        [StringLength(128)]
        public string mLongDesc;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SDSC")]
        [StringLength(32)]
        public string mShortDesc;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TYPE")]
        [StringLength(32)]
        public string mType;
        
    }
}
