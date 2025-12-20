using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct CheckoutProduct
	{

		[TdfMember("PDID")]
		public string mProductId;

		[TdfMember("PDRN")]
		public uint mQuantity;

	}
}
