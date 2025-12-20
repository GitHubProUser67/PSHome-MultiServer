using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetTosInfoRequest
    {
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("CTRY")]
        [StringLength(4)]
        public string mIsoCountryCode;
        
        /// <summary>
        /// Max String Length: 20
        /// </summary>
        [TdfMember("PTFM")]
        [StringLength(20)]
        public string mPlatform;
        
    }
}
