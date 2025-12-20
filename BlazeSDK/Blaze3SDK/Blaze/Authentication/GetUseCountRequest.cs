using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetUseCountRequest
	{

		[TdfMember("ETAG")]
		public string mEntitlementTag;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("BUID")]
		public long mUserId;

	}
}
