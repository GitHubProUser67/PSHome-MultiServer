using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct XboxServerAddress
    {
        
        [TdfMember("PORT")]
        public ushort mPort;
        
        [TdfMember("SID")]
        public uint mServiceId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SITE")]
        [StringLength(64)]
        public string mSiteName;
        
    }
}
