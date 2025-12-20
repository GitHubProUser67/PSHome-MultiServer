using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerListRequest
    {
        
        [TdfMember("CNT")]
        public uint mCount;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("ENV")]
        [StringLength(256)]
        public string mEnvironment;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PLAT")]
        [StringLength(256)]
        public string mPlatform;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PROF")]
        [StringLength(256)]
        public string mConnectionProfile;
        
    }
}
