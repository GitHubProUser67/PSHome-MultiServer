using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct PersonaDetails
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("DSNM")]
        [StringLength(256)]
        public string mDisplayName;
        
        [TdfMember("LAST")]
        public uint mLastAuthenticated;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
        [TdfMember("XREF")]
        public ulong mExtId;
        
        [TdfMember("XTYP")]
        public ExternalRefType mExtType;
        
    }
}
