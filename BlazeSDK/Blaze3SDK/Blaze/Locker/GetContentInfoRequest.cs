using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct GetContentInfoRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CID")]
		public int mContentId;

	}
}
