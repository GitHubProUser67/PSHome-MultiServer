using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ScheduledCategorySpecs
	{

		[TdfMember("CMAP")]
		public SortedDictionary<uint, ScheduledCategorySpec> mSpecMap;

	}
}
