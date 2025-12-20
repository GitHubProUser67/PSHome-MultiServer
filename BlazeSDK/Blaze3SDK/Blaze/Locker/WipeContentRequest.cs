using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct WipeContentRequest
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CIDS")]
		public List<int> mContentIds;

		[TdfMember("DTRG")]
		public DateRange mDateRange;

	}
}
