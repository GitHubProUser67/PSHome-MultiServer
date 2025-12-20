using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PostEntitlementRequest
	{

		[TdfMember("DEID")]
		public uint mDeviceId;

		[TdfMember("TAG")]
		public string mEntitlementTag;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("STAT")]
		public EntitlementStatus mStatus;

		[TdfMember("PERS")]
		public bool mWithPersona;

	}
}
