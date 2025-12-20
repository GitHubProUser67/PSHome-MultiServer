using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct League
    {
        
        /// <summary>
        /// Max String Length: 6
        /// </summary>
        [TdfMember("ABBR")]
        [StringLength(6)]
        public string mAbbrev;
        
        [TdfMember("ACTV")]
        public uint mLastActiveTime;
        
        [TdfMember("CHMP")]
        public LeagueUser mCurrChampion;
        
        [TdfMember("CREA")]
        public LeagueUser mCreator;
        
        [TdfMember("CRND")]
        public byte mCurrRound;
        
        [TdfMember("CRTM")]
        public uint mCreationTime;
        
        [TdfMember("CURM")]
        public ushort mNumMembers;
        
        /// <summary>
        /// Max String Length: 65
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(65)]
        public string mDescription;
        
        [TdfMember("DNF")]
        public short mMaxDNF;
        
        [TdfMember("GAMS")]
        public byte mNumGames;
        
        [TdfMember("ISGM")]
        public byte mIsGM;
        
        [TdfMember("LFLG")]
        public LeagueFlags mLeagueFlags;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
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
        
        [TdfMember("ONLN")]
        public byte mNumberOfMembersOnline;
        
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
        public short mMinReputation;
        
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
        
        [TdfMember("SEAS")]
        public ushort mCurrSeason;
        
        [TdfMember("SMET")]
        public byte mIsStringMetadata;
        
        [TdfMember("STRK")]
        public byte mChampionNumWins;
        
        [TdfMember("STTS")]
        public LeagueState mLeagueState;
        
        [TdfMember("TEMS")]
        public List<uint> mTeamsInUse;
        
        [TdfMember("TRPH")]
        public uint mTrophy;
        
        [TdfMember("TTYP")]
        public TradeType mTradeType;
        
        public enum LeagueState : int
        {
            LEAGUE_STATE_NONE = 0x0,
            LEAGUE_STATE_REGISTRATION = 0x1,
            LEAGUE_STATE_DRAFT = 0x2,
            LEAGUE_STATE_REGULAR_SEASON = 0x3,
            LEAGUE_STATE_PLAYOFFS = 0x4,
            LEAGUE_STATE_COMPLETED = 0x5,
        }
        
    }
}
