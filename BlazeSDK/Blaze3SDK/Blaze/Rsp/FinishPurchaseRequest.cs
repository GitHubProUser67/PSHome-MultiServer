using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct FinishPurchaseRequest
	{

		[TdfMember("PID")]
		public uint mPurchaseId;

	}
}
