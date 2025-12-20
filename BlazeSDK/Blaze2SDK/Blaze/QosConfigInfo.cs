using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct QosConfigInfo
    {
        
        [TdfMember("BWPS")]
        public QosPingSiteInfo mBandwidthPingSiteInfo;
        
        [TdfMember("LNP")]
        public ushort mNumLatencyProbes;
        
        /// <summary>
        /// Max Key String Length: 64
        /// </summary>
        [TdfMember("LTPS")]
        public SortedDictionary<string, QosPingSiteInfo> mPingSiteInfoByAliasMap;
        
        [TdfMember("SVID")]
        public uint mServiceId;
        
    }
}
