using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetCatalogsResponse
	{

		[TdfMember("CLNM")]
		public SortedDictionary<string, Catalog> mCatalogMap;

	}
}
