using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct WipeStatsRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategoryName;
        
        [TdfMember("CID")]
        public uint mContextId;
        
        [TdfMember("CTYP")]
        public uint mContextType;
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        [TdfMember("OPER")]
        public WipeStatsOperation mOperation;
        
        public enum WipeStatsOperation : int
        {
            DELETE_BY_CATEGORY_CONTEXT = 0x1,
            DELETE_BY_CATEGORY_CONTEXT_ENTITYID = 0x2,
            DELETE_BY_CONTEXT = 0x3,
            DELETE_BY_CONTEXT_ENTITYID = 0x4,
            DELETE_BY_ENTITYID = 0x5,
        }
        
    }
}
