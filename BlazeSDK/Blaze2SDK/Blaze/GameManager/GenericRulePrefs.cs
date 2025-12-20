using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GenericRulePrefs
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mRuleName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLD")]
        [StringLength(32)]
        public string mMinFitThresholdName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VALU")]
        public List<string> mDesiredValues;
        
    }
}
