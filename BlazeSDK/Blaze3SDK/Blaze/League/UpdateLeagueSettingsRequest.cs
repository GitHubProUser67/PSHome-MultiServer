using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct UpdateLeagueSettingsRequest
	{

		[TdfMember("ABBR")]
		public string mAbbrev;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("LFLG")]
		public LeagueFlags mLeagueFlags;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("LOGO")]
		public ushort mLogo;

		[TdfMember("OPTS")]
		public List<int> mOptions;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("TRPH")]
		public uint mTrophy;

	}
}
