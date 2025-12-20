using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct StatGroupList
    {
        
        [TdfMember("GRPS")]
        public List<StatGroupSummary> mGroups;
        
    }
}
