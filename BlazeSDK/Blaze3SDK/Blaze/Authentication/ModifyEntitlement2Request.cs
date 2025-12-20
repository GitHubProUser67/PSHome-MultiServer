using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ModifyEntitlement2Request
	{

		[TdfMember("EID")]
		public long mEntitlementId;

		[TdfMember("STAT")]
		public EntitlementStatus mStatus;

		[TdfMember("STRC")]
		public StatusReason mStatusReasonCode;

		[TdfMember("EXPI")]
		public string mTermination;

		[TdfMember("COUN")]
		public string mUseCount;

		[TdfMember("VERS")]
		public uint mVersion;

	}
}
