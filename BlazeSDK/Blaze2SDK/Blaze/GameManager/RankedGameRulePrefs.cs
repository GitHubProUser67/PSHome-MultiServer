using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RankedGameRulePrefs
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLD")]
        [StringLength(32)]
        public string mMinFitThresholdName;
        
        [TdfMember("VALU")]
        public RankedGameDesiredValue mDesiredRankedGameValue;
        
        public enum RankedGameDesiredValue : int
        {
            UNRANKED = 0x1,
            RANKED = 0x2,
            RANDOM = 0x4,
            ABSTAIN = 0x8,
        }
        
    }
}
