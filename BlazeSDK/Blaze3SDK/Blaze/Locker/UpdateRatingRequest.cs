using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct UpdateRatingRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("RATE")]
		public uint mRating;

	}
}
