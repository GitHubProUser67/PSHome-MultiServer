using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct ScheduledCategory
    {
        
        [TdfMember("SCHS")]
        public ScheduledCategorySpec mScheduledSpec;
        
        [TdfMember("SOID")]
        public uint mScheduledId;
        
    }
}
