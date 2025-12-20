using Blaze2SDK.Blaze.Util;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct NetworkInfo
    {

        [TdfMember("ADDR")]
        public NetworkAddress mAddress;

        /// <summary>
        /// Max Key String Length: 64
        /// </summary>
        [TdfMember("NLMP")]
        public SortedDictionary<string, int> mPingSiteLatencyByAliasMap;

        [TdfMember("NQOS")]
        public NetworkQosData mQosData;

    }
}
