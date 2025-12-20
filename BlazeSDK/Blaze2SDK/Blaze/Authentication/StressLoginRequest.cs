using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct StressLoginRequest
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        [TdfMember("NUID")]
        public ulong mNucleusId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(256)]
        public string mPersonaName;
        
    }
}
