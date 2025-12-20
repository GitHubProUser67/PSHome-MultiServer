using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UpdatePingSiteLatencyRequest
    {
        
        /// <summary>
        /// Max Key String Length: 64
        /// </summary>
        [TdfMember("NLMP")]
        public SortedDictionary<string, int> mPingSiteLatencyByAliasMap;
        
    }
}
