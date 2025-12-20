using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SkillRulePrefs
    {
        
        [TdfMember("SKDS")]
        public int mSkillValueOverride;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SKRN")]
        [StringLength(32)]
        public string mRuleName;
        
        [TdfMember("SVOR")]
        public SkillValueOverride mUseSkillValueOverride;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLD")]
        [StringLength(32)]
        public string mMinFitThresholdName;
        
    }
}
