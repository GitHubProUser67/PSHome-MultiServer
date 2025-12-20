using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetStatGroupRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
    }
}
