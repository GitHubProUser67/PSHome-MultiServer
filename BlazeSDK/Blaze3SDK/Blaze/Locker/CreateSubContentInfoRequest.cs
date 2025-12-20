using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CreateSubContentInfoRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("SUBL")]
		public List<string> mSubContentNames;

	}
}
