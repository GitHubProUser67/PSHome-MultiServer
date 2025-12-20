using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetCategories
	{

		[TdfMember("CLNM")]
		public string mCatalogName;

	}
}
