using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct StartPurchaseResponse
	{

		[TdfMember("PID")]
		public uint mPurchaseId;

	}
}
