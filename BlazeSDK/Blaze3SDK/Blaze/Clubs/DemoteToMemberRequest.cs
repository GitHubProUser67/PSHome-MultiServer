using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct DemoteToMemberRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("BLID")]
		public long mUserId;

	}
}
