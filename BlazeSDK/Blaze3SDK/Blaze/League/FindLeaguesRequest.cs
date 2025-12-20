using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct FindLeaguesRequest
	{

		[TdfMember("ABBR")]
		public string mAbbrev;

		[TdfMember("DRFT")]
		public int mDraftEnabled;

		[TdfMember("JOIN")]
		public int mJoinsEnabled;

		[TdfMember("DNF")]
		public int mMaxDNF;

		[TdfMember("MAXM")]
		public int mMaxMembers;

		[TdfMember("MAXR")]
		public uint mMaxResults;

		[TdfMember("REP")]
		public int mMinReputation;

		[TdfMember("LGNM")]
		public string mName;

		[TdfMember("GAME")]
		public int mNumGames;

		[TdfMember("OPTS")]
		public List<int> mOptions;

		[TdfMember("PSTA")]
		public int mPlayerStatsEnabled;

		[TdfMember("POFF")]
		public int mPlayoffsEnabled;

		[TdfMember("TEAM")]
		public int mPreferredTeamId;

		[TdfMember("META")]
		public byte mRetrieveMetadata;

		[TdfMember("TRAD")]
		public int mTradesEnabled;

		[TdfMember("UNIQ")]
		public int mUniqueTeamsEnabled;

	}
}
