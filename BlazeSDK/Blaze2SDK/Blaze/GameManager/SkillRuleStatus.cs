using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SkillRuleStatus
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mRuleName;
        
        [TdfMember("SKMN")]
        public int mMinSkillAccepted;
        
        [TdfMember("SKMX")]
        public int mMaxSkillAccepted;
        
    }
}
