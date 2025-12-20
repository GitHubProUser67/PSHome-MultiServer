using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct FilteredLeaderboardStatsRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ANAM")]
        [StringLength(32)]
        public string mListName;
        
        [TdfMember("CID")]
        public uint mContextId;
        
        [TdfMember("IDLS")]
        public List<uint> mListOfIds;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("KSUM")]
        public SortedDictionary<string, string> mScopeValueMap;
        
        [TdfMember("LBID")]
        public int mBoardId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mBoardName;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        [TdfMember("TIME")]
        public int mTime;
        
    }
}
