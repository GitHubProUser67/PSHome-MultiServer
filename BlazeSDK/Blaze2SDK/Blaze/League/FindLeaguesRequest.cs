using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct FindLeaguesRequest
    {
        
        /// <summary>
        /// Max String Length: 6
        /// </summary>
        [TdfMember("ABBR")]
        [StringLength(6)]
        public string mAbbrev;
        
        [TdfMember("DNF")]
        public int mMaxDNF;
        
        [TdfMember("DRFT")]
        public int mDraftEnabled;
        
        [TdfMember("GAME")]
        public int mNumGames;
        
        [TdfMember("JOIN")]
        public int mJoinsEnabled;
        
        /// <summary>
        /// Max String Length: 21
        /// </summary>
        [TdfMember("LGNM")]
        [StringLength(21)]
        public string mName;
        
        [TdfMember("MAXM")]
        public int mMaxMembers;
        
        [TdfMember("MAXR")]
        public uint mMaxResults;
        
        [TdfMember("META")]
        public byte mRetrieveMetadata;
        
        [TdfMember("OPTS")]
        public List<int> mOptions;
        
        [TdfMember("POFF")]
        public int mPlayoffsEnabled;
        
        [TdfMember("PSTA")]
        public int mPlayerStatsEnabled;
        
        [TdfMember("REP")]
        public int mMinReputation;
        
        [TdfMember("TEAM")]
        public int mPreferredTeamId;
        
        [TdfMember("TRAD")]
        public int mTradesEnabled;
        
        [TdfMember("UNIQ")]
        public int mUniqueTeamsEnabled;
        
    }
}
