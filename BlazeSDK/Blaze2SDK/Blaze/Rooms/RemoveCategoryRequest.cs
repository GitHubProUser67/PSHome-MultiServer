using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RemoveCategoryRequest
    {
        
        [TdfMember("CTID")]
        public uint mCategoryId;
        
    }
}
