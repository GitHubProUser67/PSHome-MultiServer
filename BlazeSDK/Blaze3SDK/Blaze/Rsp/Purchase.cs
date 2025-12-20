using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct Purchase
	{

		[TdfMember("CID")]
		public uint mConsumableId;

		[TdfMember("SITE")]
		public string mPingSiteAlias;

		[TdfMember("PID")]
		public uint mPurchaseId;

		[TdfMember("QUAN")]
		public uint mQuantity;

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("STAT")]
		public PurchaseStatus mStatus;

		[TdfMember("UPDA")]
		public TimeValue mUpdatedDate;

		[TdfMember("UID")]
		public long mUserId;

	}
}
