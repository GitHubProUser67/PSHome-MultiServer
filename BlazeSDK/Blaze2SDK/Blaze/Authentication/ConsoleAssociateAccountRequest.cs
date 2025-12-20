using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ConsoleAssociateAccountRequest
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(64)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(256)]
        public string mExtName;
        
        [TdfMember("TICK")]
        public byte[] mTicketBlob;
        
        [TdfMember("XREF")]
        public ulong mExtId;
        
    }
}
