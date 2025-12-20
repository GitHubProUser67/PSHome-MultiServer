using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetProductsResponse
	{

		[TdfMember("PDRL")]
		public List<Product> mProductVector;

	}
}
