using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct StatUpdate
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("TYPE")]
        public int mUpdateType;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VALU")]
        [StringLength(256)]
        public string mValue;
        
    }
}
