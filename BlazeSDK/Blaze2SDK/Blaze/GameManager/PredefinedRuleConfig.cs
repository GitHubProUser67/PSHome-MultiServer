using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct PredefinedRuleConfig
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("RNME")]
        [StringLength(32)]
        public string mRuleName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLS")]
        public List<string> mThresholdNames;
        
        [TdfMember("WGHT")]
        public uint mWeight;
        
    }
}
