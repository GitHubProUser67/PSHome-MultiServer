using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct FetchClientConfigRequest
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CFID")]
        [StringLength(64)]
        public string mConfigSection;
        
    }
}
