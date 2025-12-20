using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct SetOwnerGroupRequest
	{

		[TdfMember("CCAT")]
		public string mCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("GPID")]
		public BlazeObjectId mGroupId;

	}
}
