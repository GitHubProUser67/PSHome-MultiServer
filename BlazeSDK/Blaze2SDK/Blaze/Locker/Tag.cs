using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct Tag
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TAG")]
        [StringLength(32)]
        public string mtag;
        
        [TdfMember("TGID")]
        public int mTagId;
        
    }
}
