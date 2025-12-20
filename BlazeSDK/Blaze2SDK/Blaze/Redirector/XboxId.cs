using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct XboxId
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GTAG")]
        [StringLength(32)]
        public string mGamertag;
        
        [TdfMember("XUID")]
        public ulong mXuid;
        
    }
}
