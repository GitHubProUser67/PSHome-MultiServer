using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct NameRemapEntry
    {
        
        [TdfMember("DPRT")]
        public ushort mDstPort;
        
        [TdfMember("SID")]
        public uint mServiceId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SIP")]
        [StringLength(64)]
        public string mHostname;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SITE")]
        [StringLength(64)]
        public string mSiteName;
        
        [TdfMember("SPRT")]
        public ushort mSrcPort;
        
    }
}
