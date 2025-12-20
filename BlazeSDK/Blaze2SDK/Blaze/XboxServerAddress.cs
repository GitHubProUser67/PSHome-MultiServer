using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct XboxServerAddress
    {
        
        [TdfMember("PORT")]
        public ushort Port;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SITE")]
        [StringLength(64)]
        public string SiteName;
        
        [TdfMember("SVID")]
        public uint Sid;
        
    }
}
