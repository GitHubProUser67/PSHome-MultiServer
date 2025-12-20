using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct Attribute
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VALU")]
        [StringLength(256)]
        public string mValue;
        
    }
}
