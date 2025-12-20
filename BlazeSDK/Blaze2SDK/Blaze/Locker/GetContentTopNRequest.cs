using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct GetContentTopNRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("COUN")]
        public int mCount;
        
        [TdfMember("FLTR")]
        public List<LeaderboardFilter> mFilters;
        
        [TdfMember("LB")]
        public LeaderboardType mLeaderboardType;
        
        [TdfMember("STRT")]
        public int mStart;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TAG")]
        [StringLength(32)]
        public string mTag;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("VIEW")]
        [StringLength(32)]
        public string mLeaderboardView;
        
    }
}
