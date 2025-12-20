using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct UpdateStatsRequest
    {
        
        [TdfMember("UPDT")]
        public List<StatRowUpdate> mStatUpdates;
        
    }
}
