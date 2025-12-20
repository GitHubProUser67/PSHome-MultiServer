using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Mail
{
    [TdfStruct]
    public struct SendMailToSelfRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("MNAP")]
        public SortedDictionary<string, string> mVariableValueMap;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("MTPL")]
        [StringLength(64)]
        public string mPurpose;
        
    }
}
