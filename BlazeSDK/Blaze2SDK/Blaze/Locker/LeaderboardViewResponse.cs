using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct LeaderboardViewResponse
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
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
        
        [TdfMember("LB")]
        public LeaderboardType mLeaderboardType;
        
        [TdfMember("LIST")]
        public List<LeaderboardViewColumn> mViewColumns;
        
        [TdfMember("SIZE")]
        public int mSize;
        
        [TdfMember("TAGS")]
        public int mTagsIncluded;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("VIEW")]
        [StringLength(32)]
        public string mLeaderboardView;
        
    }
}
