using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct Invitation
	{

		[TdfMember("CRTM")]
		public uint mCreationTime;

		[TdfMember("INVT")]
		public LeagueUser mInvitee;

		[TdfMember("MEMB")]
		public LeagueUser mInviter;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("META")]
		public byte[] mMetadata;

	}
}
