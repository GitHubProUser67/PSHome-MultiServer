using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GenericRuleStatus
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mRuleName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VALU")]
        public List<string> mMatchedValues;
        
    }
}
