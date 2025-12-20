using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetStatResponse
    {
        
        /// <summary>
        /// Max Key String Length: 1024
        /// </summary>
        [TdfMember("KSSV")]
        public SortedDictionary<string, StatValues> mKeyScopeStatsValueMap;
        
    }
}
