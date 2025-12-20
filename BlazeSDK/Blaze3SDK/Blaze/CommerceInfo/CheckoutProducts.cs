using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct CheckoutProducts
	{

		[TdfMember("PDLS")]
		public List<CheckoutProduct> mCheckoutProducts;

		[TdfMember("WLNM")]
		public string mWalletName;

	}
}
