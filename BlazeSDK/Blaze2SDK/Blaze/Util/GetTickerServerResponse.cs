using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct GetTickerServerResponse
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ADRS")]
        [StringLength(64)]
        public string mAddress;
        
        [TdfMember("PORT")]
        public uint mPort;
        
        /// <summary>
        /// Max String Length: 512
        /// </summary>
        [TdfMember("SKEY")]
        [StringLength(512)]
        public string mKey;
        
    }
}
