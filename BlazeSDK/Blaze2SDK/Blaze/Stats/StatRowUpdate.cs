using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct StatRowUpdate
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        [TdfMember("CID")]
        public uint mContextId;
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("KSUM")]
        public SortedDictionary<string, uint> mKeyScopeIndexMap;
        
        [TdfMember("PTYP")]
        public List<int> mPeriodTypes;
        
        [TdfMember("UPDT")]
        public List<StatUpdate> mUpdates;
        
    }
}
