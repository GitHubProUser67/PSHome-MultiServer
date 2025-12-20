using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetCatalogsResponse
    {
        
        /// <summary>
        /// Max Key String Length: 255
        /// </summary>
        [TdfMember("CLNM")]
        public SortedDictionary<string, Catalog> mCatalogMap;
        
    }
}
