using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetTosInfoResponse
    {
        
        [TdfMember("EAMC")]
        public uint mEaMayContact;
        
        [TdfMember("PMC")]
        public uint mPartnersMayContact;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("PRIV")]
        [StringLength(128)]
        public string mPrivacyPolicyUri;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("THST")]
        [StringLength(128)]
        public string mTosHost;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("TURI")]
        [StringLength(128)]
        public string mTosUri;
        
    }
}
