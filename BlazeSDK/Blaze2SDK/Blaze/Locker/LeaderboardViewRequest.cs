using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct LeaderboardViewRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("LB")]
        public LeaderboardType mLeaderboardType;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("VIEW")]
        [StringLength(32)]
        public string mLeaderboardView;
        
    }
}
