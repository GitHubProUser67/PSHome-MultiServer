using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct RemoveMemberRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("BLID")]
		public long mUserId;

	}
}
