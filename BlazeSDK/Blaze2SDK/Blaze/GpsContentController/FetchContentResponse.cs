using Tdf;

namespace Blaze2SDK.Blaze.GpsContentController
{
    [TdfStruct]
    public struct FetchContentResponse
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ANVP")]
        public SortedDictionary<string, string> attributeMap;
        
    }
}
