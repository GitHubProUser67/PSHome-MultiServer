using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetStatsByGroupRequest
    {
        
        [TdfMember("AGGR")]
        public AggregateCalcFlags mAggrFlags;
        
        [TdfMember("CID")]
        public uint mContextId;
        
        [TdfMember("EID")]
        public List<uint> mEntityIds;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("KSUM")]
        public SortedDictionary<string, string> mKeyScopeUnitMap;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mGroupName;
        
        [TdfMember("PCTR")]
        public int mPeriodCtr;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        [TdfMember("PTYP")]
        public int mPeriodType;
        
        [TdfMember("TIME")]
        public int mTime;
        
        [TdfMember("VID")]
        public uint mViewId;
        
    }
}
