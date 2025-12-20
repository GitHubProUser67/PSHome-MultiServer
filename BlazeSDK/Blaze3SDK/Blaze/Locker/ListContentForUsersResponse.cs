using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ListContentForUsersResponse
	{

		[TdfMember("LKRS")]
		public SortedDictionary<long, ContentInfos> mContentInfoMap;

		[TdfMember("MSIZ")]
		public int mSizeAllowed;

		[TdfMember("TCNT")]
		public uint mTotalCount;

	}
}
