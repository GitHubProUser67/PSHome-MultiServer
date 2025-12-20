using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetInvitationsRequest
	{

		[TdfMember("INVT")]
		public InvitationsToGetType mInvitationsToGet;

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
