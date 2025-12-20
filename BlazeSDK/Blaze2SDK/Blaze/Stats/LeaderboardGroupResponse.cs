using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardGroupResponse
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("BNAM")]
        [StringLength(64)]
        public string mBoardName;
        
        [TdfMember("CTYP")]
        public uint mContextType;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(128)]
        public string mDesc;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("KSUM")]
        public SortedDictionary<string, string> mKeyScopeUnitMap;
        
        [TdfMember("LBSZ")]
        public int mLeaderboardSize;
        
        [TdfMember("LIST")]
        public List<LeaderboardStatKeyColumn> mStatKeyColumns;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("META")]
        [StringLength(128)]
        public string mMetadata;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SNAM")]
        [StringLength(32)]
        public string mStatName;
        
    }
}
