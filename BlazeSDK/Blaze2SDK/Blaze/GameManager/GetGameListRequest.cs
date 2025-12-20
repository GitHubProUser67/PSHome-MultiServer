using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GetGameListRequest
    {
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("DNAM")]
        [StringLength(16)]
        public string mListConfigName;
        
        [TdfMember("GLID")]
        public MatchmakingCriteriaData mListCriteria;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GVER")]
        [StringLength(64)]
        public string mGameProtocolVersionString;
        
        [TdfMember("IGNO")]
        public bool mIgnoreGameEntryCriteria;
        
        [TdfMember("LCAP")]
        public uint mListCapacity;
        
        [TdfMember("NOJM")]
        public bool mIgnoreGameJoinMethod;
        
    }
}
