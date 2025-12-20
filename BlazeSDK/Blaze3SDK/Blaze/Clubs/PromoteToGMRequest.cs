using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct PromoteToGMRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("BLID")]
		public long mUserId;

	}
}
