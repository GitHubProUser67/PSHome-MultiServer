using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetStatsRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        [TdfMember("CID")]
        public uint mContextId;
        
        [TdfMember("CTYP")]
        public uint mContextType;
        
        [TdfMember("EID")]
        public List<uint> mEntityIds;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("KSLS")]
        public SortedDictionary<string, string> mKeyScopeUnitMap;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        public List<string> mStatNames;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        [TdfMember("PTYP")]
        public int mPeriodType;
        
    }
}
