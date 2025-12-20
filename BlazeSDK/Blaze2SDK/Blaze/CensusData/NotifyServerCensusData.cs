using Blaze3SDK.Blaze;
using Tdf;

namespace Blaze2SDK.Blaze.CensusData
{
    [TdfStruct]
    public struct NotifyServerCensusData
    {

        [TdfMember("CDBI")]
        public SortedDictionary<uint, CensusValue> mCensusDataByIndexMap;

    }
}
