using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SetDefaultRosterRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("ROST")]
		public string mRosterId;

		[TdfMember("TEAM")]
		public uint mTeamId;

		[TdfMember("USER")]
		public long mUserId;

	}
}
