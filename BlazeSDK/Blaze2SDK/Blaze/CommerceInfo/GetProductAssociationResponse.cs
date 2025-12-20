using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetProductAssociationResponse
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CCDT")]
        [StringLength(255)]
        public string mCreateDate;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CMDT")]
        [StringLength(255)]
        public string mModifiedDate;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CPDN")]
        [StringLength(255)]
        public string mProductName;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CPJN")]
        [StringLength(255)]
        public string mProjectNumber;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CSER")]
        [StringLength(255)]
        public string mExternalRef;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CSFN")]
        [StringLength(255)]
        public string mFileName;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CSN")]
        [StringLength(255)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CST")]
        [StringLength(255)]
        public string mType;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CSTA")]
        [StringLength(255)]
        public string mStatus;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CURI")]
        [StringLength(255)]
        public string mUri;
        
        [TdfMember("PLST")]
        public List<ProductAssociation> mProductAssociationList;
        
        [TdfMember("UID")]
        public ulong mId;
        
    }
}
