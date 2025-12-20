using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetCategoriesResponse
    {
        
        /// <summary>
        /// Max Key String Length: 255
        /// </summary>
        [TdfMember("CLNM")]
        public SortedDictionary<string, Category> mCategoryMap;
        
    }
}
