using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct RemoveCachedClubRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("UPRS")]
		public UpdateReason mReason;

	}
}
