using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct PingSiteRuleStatus
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VALU")]
        public List<string> mMatchedValues;
        
    }
}
