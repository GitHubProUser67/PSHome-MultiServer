using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct PlaygroupCensusData
    {
        
        [TdfMember("PGN")]
        public uint mNumOfPlaygroup;
        
        [TdfMember("PIPN")]
        public uint mNumOfPlayersInPlaygroup;
        
    }
}
