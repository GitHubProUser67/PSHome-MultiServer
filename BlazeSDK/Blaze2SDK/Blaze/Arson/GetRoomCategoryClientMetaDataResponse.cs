using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct GetRoomCategoryClientMetaDataResponse
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mAttributes;
        
    }
}
