using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerEndpointInfo
    {
        
        [TdfMember("ADRS")]
        public List<ServerAddressInfo> mAddresses;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CHAN")]
        [StringLength(32)]
        public string mChannel;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DEC")]
        [StringLength(32)]
        public string mDecoder;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ENC")]
        [StringLength(32)]
        public string mEncoder;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("PROT")]
        [StringLength(32)]
        public string mProtocol;
        
    }
}
