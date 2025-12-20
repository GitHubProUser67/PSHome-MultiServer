using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardStatValues
    {
        
        [TdfMember("LDLS")]
        public List<LeaderboardStatValuesRow> mRows;
        
    }
}
