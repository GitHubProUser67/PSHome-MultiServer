using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ConsumecodeResponse
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("EXRF")]
        [StringLength(128)]
        public string mExtRef;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("KEY")]
        [StringLength(128)]
        public string mKeyCode;
        
        [TdfMember("MCNT")]
        public long mMultiUseCount;
        
        [TdfMember("MFLG")]
        public byte mMultiUseFlag;
        
        [TdfMember("MLMT")]
        public long mMultiUseLimit;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("PRID")]
        [StringLength(128)]
        public string mProductId;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("PRMN")]
        [StringLength(128)]
        public string mProductName;
        
        [TdfMember("STAT")]
        public KeymasterCodeStatus mStatus;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
