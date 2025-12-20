using Tdf;

namespace Blaze2SDK.Blaze.CensusData
{
    [TdfStruct]
    public struct RegionCounts
    {
        
        /// <summary>
        /// Max Key String Length: 12
        /// </summary>
        [TdfMember("CNOU")]
        public SortedDictionary<string, uint> mNumOfUsersByRegion;
        
    }
}
