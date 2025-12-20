using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetDateRangeRequest
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("PTYP")]
		public int mPeriodType;

	}
}
