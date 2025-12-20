using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct CreateLeagueRequest
	{

		[TdfMember("ABBR")]
		public string mAbbrev;

		[TdfMember("TEAM")]
		public uint mCreatorTeamId;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("ROST")]
		public string mGameTeamRoster;

		[TdfMember("SMET")]
		public byte mIsStringMetadata;

		[TdfMember("LFLG")]
		public LeagueFlags mLeagueFlags;

		[TdfMember("LOGO")]
		public ushort mLogo;

		[TdfMember("DNF")]
		public int mMaxDNF;

		[TdfMember("MAXM")]
		public ushort mMaxMembers;

		[TdfMember("META")]
		public byte[] mMetadata;

		[TdfMember("REP")]
		public int mMinReputation;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("GAMS")]
		public byte mNumGames;

		[TdfMember("PLON")]
		public byte mNumPlayoffGames;

		[TdfMember("RNDS")]
		public byte mNumRounds;

		[TdfMember("OPTS")]
		public List<int> mOptions;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("PTYP")]
		public PlayoffType mPlayoffType;

		[TdfMember("SCHD")]
		public ScheduleType mScheduleType;

		[TdfMember("TTYP")]
		public TradeType mTradeType;

		[TdfMember("TRPH")]
		public uint mTrophy;

	}
}
