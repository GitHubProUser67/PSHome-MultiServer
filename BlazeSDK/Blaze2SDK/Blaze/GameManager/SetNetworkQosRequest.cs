using Blaze2SDK.Blaze.Util;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SetNetworkQosRequest
    {

        [TdfMember("NQOS")]
        public NetworkQosData mNetworkQosData;

    }
}
