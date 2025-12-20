using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CategoryListResponse
	{

		[TdfMember("LIST")]
		public List<CategorySummary> mCategorySummaries;

	}
}
