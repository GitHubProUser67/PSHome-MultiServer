using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct PredefinedPingSiteRuleConfig
    {
        
        [TdfMember("PDRC")]
        public PredefinedRuleConfig mPredefinedRuleConfig;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("POSV")]
        public List<string> mPossibleValues;
        
    }
}
