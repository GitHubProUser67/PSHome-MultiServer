using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportColumn
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ANAM")]
        [StringLength(32)]
        public string mAttributeName;
        
        [TdfMember("ATID")]
        public uint mIndex;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ATYP")]
        [StringLength(64)]
        public string mAttributeType;
        
        [TdfMember("DTYP")]
        public int mType;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
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
        public string mDesc;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("META")]
        [StringLength(128)]
        public string mMetadata;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SDSC")]
        [StringLength(32)]
        public string mShortDesc;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VALU")]
        public List<string> mValues;
        
    }
}
