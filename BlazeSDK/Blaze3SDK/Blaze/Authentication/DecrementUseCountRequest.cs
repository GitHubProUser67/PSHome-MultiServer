using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct DecrementUseCountRequest
	{

		[TdfMember("DEUC")]
		public uint mDecrementCount;

		[TdfMember("ETAG")]
		public string mEntitlementTag;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

	}
}
