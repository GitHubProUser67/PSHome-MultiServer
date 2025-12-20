using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct League
	{

		[TdfMember("ABBR")]
		public string mAbbrev;

		[TdfMember("STRK")]
		public byte mChampionNumWins;

		[TdfMember("CRTM")]
		public uint mCreationTime;

		[TdfMember("CREA")]
		public LeagueUser mCreator;

		[TdfMember("CHMP")]
		public LeagueUser mCurrChampion;

		[TdfMember("CRND")]
		public byte mCurrRound;

		[TdfMember("SEAS")]
		public ushort mCurrSeason;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("ROST")]
		public string mGameTeamRoster;

		[TdfMember("ISGM")]
		public byte mIsGM;

		[TdfMember("SMET")]
		public byte mIsStringMetadata;

		[TdfMember("ACTV")]
		public uint mLastActiveTime;

		[TdfMember("LFLG")]
		public LeagueFlags mLeagueFlags;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("STTS")]
		public LeagueState mLeagueState;

		[TdfMember("LOGO")]
		public ushort mLogo;

		[TdfMember("DNF")]
		public short mMaxDNF;

		[TdfMember("MAXM")]
		public ushort mMaxMembers;

		[TdfMember("META")]
		public byte[] mMetadata;

		[TdfMember("REP")]
		public short mMinReputation;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("GAMS")]
		public byte mNumGames;

		[TdfMember("CURM")]
		public ushort mNumMembers;

		[TdfMember("PLON")]
		public byte mNumPlayoffGames;

		[TdfMember("RNDS")]
		public byte mNumRounds;

		[TdfMember("ONLN")]
		public byte mNumberOfMembersOnline;

		[TdfMember("OPTS")]
		public List<int> mOptions;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("PTYP")]
		public PlayoffType mPlayoffType;

		[TdfMember("SCHD")]
		public ScheduleType mScheduleType;

		[TdfMember("TEMS")]
		public List<uint> mTeamsInUse;

		[TdfMember("TTYP")]
		public TradeType mTradeType;

		[TdfMember("TRPH")]
		public uint mTrophy;

	}
}
