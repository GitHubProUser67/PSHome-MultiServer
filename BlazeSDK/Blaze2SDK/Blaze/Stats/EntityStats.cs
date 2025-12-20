using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct EntityStats
    {
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("STAT")]
        public List<string> mStatValues;
        
    }
}
