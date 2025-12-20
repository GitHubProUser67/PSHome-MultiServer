using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatCategoryList
	{

		[TdfMember("CATS")]
		public List<StatCategorySummary> mCategories;

	}
}
