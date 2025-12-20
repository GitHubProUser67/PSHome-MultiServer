using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct DeleteContentRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CIDS")]
		public List<int> mContentId;

	}
}
