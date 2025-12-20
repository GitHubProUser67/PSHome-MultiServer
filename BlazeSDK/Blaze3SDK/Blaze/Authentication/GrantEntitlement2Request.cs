using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GrantEntitlement2Request
	{

		[TdfMember("DEID")]
		public string mDeviceId;

		[TdfMember("TAG")]
		public string mEntitlementTag;

		[TdfMember("TYPE")]
		public EntitlementType mEntitlementType;

		[TdfMember("GDAY")]
		public string mGrantDate;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("ISSE")]
		public bool mIsSearch;

		[TdfMember("MALI")]
		public bool mManagedLifecycle;

		[TdfMember("PEID")]
		public long mPersonaId;

		[TdfMember("PRCA")]
		public ProductCatalog mProductCatalog;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("STAT")]
		public EntitlementStatus mStatus;

		[TdfMember("STRE")]
		public StatusReason mStatusReasonCode;

		[TdfMember("EXPI")]
		public string mTermination;

		[TdfMember("COUN")]
		public string mUseCount;

		[TdfMember("BUID")]
		public long mUserId;

		[TdfMember("PERS")]
		public bool mWithPersona;

	}
}
