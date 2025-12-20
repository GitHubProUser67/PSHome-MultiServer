using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ConsoleCreateAccountRequest
    {
        
        [TdfMember("CREQ")]
        public CreateAccountParameters mCreateAccountParameters;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(256)]
        public string mExtName;
        
        [TdfMember("TICK")]
        public byte[] mTicketBlob;
        
        [TdfMember("UID")]
        public long mUserId;
        
        [TdfMember("XREF")]
        public ulong mExtId;
        
    }
}
