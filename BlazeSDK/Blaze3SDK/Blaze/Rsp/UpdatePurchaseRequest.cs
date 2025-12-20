using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdatePurchaseRequest
	{

		[TdfMember("PID")]
		public uint mPurchaseId;

		[TdfMember("QUAN")]
		public uint mQuantity;

		[TdfMember("STAT")]
		public PurchaseStatus mStatus;

	}
}
