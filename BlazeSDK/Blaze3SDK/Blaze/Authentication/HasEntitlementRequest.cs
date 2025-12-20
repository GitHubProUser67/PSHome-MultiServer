using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct HasEntitlementRequest
	{

		[TdfMember("DEID")]
		public uint mDeviceId;

		[TdfMember("FLAG")]
		public EntitlementSearchFlag mEntitlementSearchFlag;

		[TdfMember("ETAG")]
		public string mEntitlementTag;

		[TdfMember("TYPE")]
		public EntitlementType mEntitlementType;

		[TdfMember("GNLS")]
		public List<string> mGroupNameList;

		[TdfMember("PID")]
		public long mPersonaId;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("BUID")]
		public long mUserId;

	}
}
