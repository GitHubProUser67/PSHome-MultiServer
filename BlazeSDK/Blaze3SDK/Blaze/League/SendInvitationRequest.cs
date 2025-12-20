using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SendInvitationRequest
	{

		[TdfMember("INVT")]
		public long mInviteeId;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("META")]
		public byte[] mMetadata;

	}
}
