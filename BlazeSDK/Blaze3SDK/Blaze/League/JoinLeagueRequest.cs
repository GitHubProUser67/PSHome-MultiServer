using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct JoinLeagueRequest
	{

		[TdfMember("DNF")]
		public int mDNF;

		[TdfMember("MMBR")]
		public long mInviter;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("TEAM")]
		public uint mTeamId;

	}
}
