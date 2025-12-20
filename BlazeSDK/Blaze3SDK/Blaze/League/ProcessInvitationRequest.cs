using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct ProcessInvitationRequest
	{

		[TdfMember("INVT")]
		public long mInviteeId;

		[TdfMember("INVR")]
		public long mInviterId;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("OPER")]
		public InvitationOp mOperation;

	}
}
