using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct MemberStats
    {
        
        [TdfMember("GOLA")]
        public int mGoalsAgainst;
        
        [TdfMember("GOLF")]
        public int mGoalsFor;
        
        [TdfMember("LOSS")]
        public int mLosses;
        
        /// <summary>
        /// Max String Length: 20
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(20)]
        public string mPersona;
        
        [TdfMember("PNTS")]
        public int mPoints;
        
        [TdfMember("RANK")]
        public int mRank;
        
        [TdfMember("TIES")]
        public int mTies;
        
        [TdfMember("WINS")]
        public int mWins;
        
    }
}
