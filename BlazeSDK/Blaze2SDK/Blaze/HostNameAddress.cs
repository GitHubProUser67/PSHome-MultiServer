using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct HostNameAddress
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string HostName;
        
        [TdfMember("PORT")]
        public ushort Port;
        
    }
}
