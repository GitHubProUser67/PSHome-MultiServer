using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetCategoriesResponse
	{

		[TdfMember("CLNM")]
		public SortedDictionary<string, Category> mCategoryMap;

	}
}
