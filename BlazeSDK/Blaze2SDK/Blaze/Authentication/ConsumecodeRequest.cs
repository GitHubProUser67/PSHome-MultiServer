using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ConsumecodeRequest
    {
        
        [TdfMember("CDKY")]
        public bool mIsCdKey;
        
        [TdfMember("CNID")]
        public byte[] mConsoleId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(64)]
        public string mGroupName;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("KEY")]
        [StringLength(128)]
        public string mCode;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PID")]
        [StringLength(64)]
        public string mProductId;
        
        [TdfMember("PNID")]
        public bool mIsBindPersona;
        
    }
}
