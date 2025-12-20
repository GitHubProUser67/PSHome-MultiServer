using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct RemoveCachedMemberRequest
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("UPRS")]
		public UpdateReason mReason;

	}
}
