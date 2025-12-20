using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct FetchConfigResponse
    {
        
        /// <summary>
        /// Max Key String Length: 64
        /// Max Value String Length: 512
        /// </summary>
        [TdfMember("CONF")]
        public SortedDictionary<string, string> mConfig;
        
    }
}
