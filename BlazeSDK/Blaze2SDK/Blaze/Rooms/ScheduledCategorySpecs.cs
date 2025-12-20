using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct ScheduledCategorySpecs
    {
        
        [TdfMember("CMAP")]
        public SortedDictionary<uint, ScheduledCategorySpec> mSpecMap;
        
    }
}
