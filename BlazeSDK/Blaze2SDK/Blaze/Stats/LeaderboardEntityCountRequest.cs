using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardEntityCountRequest
    {
        
        [TdfMember("CID")]
        public uint mContextId;
        
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
        
    }
}
