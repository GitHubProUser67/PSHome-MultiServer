using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetPersonaByIdResponse
    {
        
        [TdfMember("BUID")]
        public uint mBlazeUserId;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(256)]
        public string mPersonaName;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
