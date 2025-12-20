using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardStatKeyColumn
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("FRMT")]
        [StringLength(32)]
        public string mFormat;
        
        /// <summary>
        /// Max String Length: 8
        /// </summary>
        [TdfMember("KIND")]
        [StringLength(8)]
        public string mKind;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("LDSC")]
        [StringLength(128)]
        public string mLongDesc;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("META")]
        [StringLength(128)]
        public string mMetadata;
        
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
        
        [TdfMember("TYPE")]
        public int mType;
        
    }
}
