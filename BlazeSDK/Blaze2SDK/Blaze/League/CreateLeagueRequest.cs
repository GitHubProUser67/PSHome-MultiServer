using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct CreateLeagueRequest
    {
        
        /// <summary>
        /// Max String Length: 6
        /// </summary>
        [TdfMember("ABBR")]
        [StringLength(6)]
        public string mAbbrev;
        
        /// <summary>
        /// Max String Length: 65
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(65)]
        public string mDescription;
        
        [TdfMember("DNF")]
        public int mMaxDNF;
        
        [TdfMember("GAMS")]
        public byte mNumGames;
        
        [TdfMember("LFLG")]
        public LeagueFlags mLeagueFlags;
        
        [TdfMember("LOGO")]
        public ushort mLogo;
        
        [TdfMember("MAXM")]
        public ushort mMaxMembers;
        
        [TdfMember("META")]
        public byte[] mMetadata;
        
        /// <summary>
        /// Max String Length: 21
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(21)]
        public string mName;
        
        [TdfMember("OPTS")]
        public List<uint> mOptions;
        
        /// <summary>
        /// Max String Length: 13
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(13)]
        public string mPassword;
        
        [TdfMember("PLON")]
        public byte mNumPlayoffGames;
        
        [TdfMember("PTYP")]
        public PlayoffType mPlayoffType;
        
        [TdfMember("REP")]
        public int mMinReputation;
        
        [TdfMember("RNDS")]
        public byte mNumRounds;
        
        /// <summary>
        /// Max String Length: 257
        /// </summary>
        [TdfMember("ROST")]
        [StringLength(257)]
        public string mGameTeamRoster;
        
        [TdfMember("SCHD")]
        public ScheduleType mScheduleType;
        
        [TdfMember("SMET")]
        public byte mIsStringMetadata;
        
        [TdfMember("TEAM")]
        public uint mCreatorTeamId;
        
        [TdfMember("TRPH")]
        public uint mTrophy;
        
        [TdfMember("TTYP")]
        public TradeType mTradeType;
        
    }
}
