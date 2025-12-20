using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardStatValuesRow
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ENAM")]
        [StringLength(64)]
        public string mEntityName;
        
        [TdfMember("ENID")]
        public uint mEntityId;
        
        [TdfMember("RANK")]
        public int mRank;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("RSTA")]
        [StringLength(256)]
        public string mRankedStat;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("STAT")]
        public List<string> mOtherStats;
        
        [TdfMember("UATT")]
        public ulong mAttribute;
        
    }
}
