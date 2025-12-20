using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ListContentResponse
	{

		[TdfMember("LKRS")]
		public List<ContentInfo> mContentInfo;

		[TdfMember("MSIZ")]
		public int mSizeAllowed;

		[TdfMember("TCNT")]
		public uint mTotalCount;

	}
}
