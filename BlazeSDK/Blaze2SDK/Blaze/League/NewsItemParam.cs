using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct NewsItemParam
    {
        
        [TdfMember("TYPE")]
        public NewsParamType mType;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("VAL")]
        [StringLength(32)]
        public string mValue;
        
    }
}
