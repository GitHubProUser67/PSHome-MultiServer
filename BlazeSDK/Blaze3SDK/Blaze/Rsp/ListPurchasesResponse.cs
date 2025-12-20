using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct ListPurchasesResponse
	{

		[TdfMember("PLST")]
		public List<Purchase> mPurchaseList;

	}
}
