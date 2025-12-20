using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct Invitations
	{

		[TdfMember("INVS")]
		public List<Invitation> mInvitations;

	}
}
