using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct Entitlement
	{

		[TdfMember("DEVI")]
		public string mDeviceUri;

		[TdfMember("TAG")]
		public string mEntitlementTag;

		[TdfMember("TYPE")]
		public EntitlementType mEntitlementType;

		[TdfMember("GDAY")]
		public string mGrantDate;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("ID")]
		public ulong mId;

		[TdfMember("ISCO")]
		public bool mIsConsumable;

		[TdfMember("PID")]
		public long mPersonaId;

		[TdfMember("PRCA")]
		public ProductCatalog mProductCatalog;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("STAT")]
		public EntitlementStatus mStatus;

		[TdfMember("STRC")]
		public StatusReason mStatusReasonCode;

		[TdfMember("TDAY")]
		public string mTerminationDate;

		[TdfMember("UCNT")]
		public uint mUseCount;

		[TdfMember("VER")]
		public uint mVersion;

	}
}
