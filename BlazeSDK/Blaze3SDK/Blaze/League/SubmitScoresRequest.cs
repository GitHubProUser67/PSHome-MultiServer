using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SubmitScoresRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("SCR0")]
		public uint mScore0;

		[TdfMember("SCR1")]
		public uint mScore1;

		[TdfMember("SIMU")]
		public byte mSimulated;

		[TdfMember("USR0")]
		public long mUser0;

		[TdfMember("USR1")]
		public long mUser1;

		[TdfMember("WLT0")]
		public uint mWlt0;

		[TdfMember("WLT1")]
		public uint mWlt1;

		[TdfMember("WTP0")]
		public uint mWtyp0;

		[TdfMember("WTP1")]
		public uint mWtyp1;

	}
}
