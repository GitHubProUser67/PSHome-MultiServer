using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct TransferOwnershipRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("OOSN")]
		public MembershipStatus mExOwnersNewStatus;

		[TdfMember("BLID")]
		public long mUserId;

	}
}
