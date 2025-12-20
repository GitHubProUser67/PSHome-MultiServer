using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct MemberInfo
	{

		[TdfMember("GREM")]
		public byte mGamesRemaining;

		[TdfMember("ISDP")]
		public byte mIsDraftProfileSubmitted;

		[TdfMember("ISGM")]
		public byte mIsGM;

		[TdfMember("ISON")]
		public byte mIsOnline;

		[TdfMember("SMET")]
		public byte mIsStringMetadata;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("MMBR")]
		public LeagueUser mMember;

		[TdfMember("META")]
		public byte[] mMetadata;

		[TdfMember("STAT")]
		public MemberStats mStats;

		[TdfMember("TEAM")]
		public uint mTeamId;

	}
}
