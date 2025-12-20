using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct UserText
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("UTXT")]
        [StringLength(1024)]
        public string mText;
        
    }
}
