using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct RemoveOwnerGroupRequest
	{

		[TdfMember("CCAT")]
		public string mCategory;

		[TdfMember("CID")]
		public int mContentId;

	}
}
