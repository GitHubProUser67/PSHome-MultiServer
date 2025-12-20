using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct UpdatePersonaRequest
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("DSNM")]
        [StringLength(256)]
        public string mDisplayName;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
    }
}
