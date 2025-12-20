using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct StatDescs
    {
        
        [TdfMember("CTYP")]
        public uint mContextType;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        [TdfMember("STAT")]
        public List<StatDescSummary> mStatDescs;
        
    }
}
