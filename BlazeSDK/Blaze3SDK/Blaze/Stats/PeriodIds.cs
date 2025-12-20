using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct PeriodIds
	{

		[TdfMember("DLY")]
		public int mCurrentDailyPeriodId;

		[TdfMember("MLY")]
		public int mCurrentMonthlyPeriodId;

		[TdfMember("WLY")]
		public int mCurrentWeeklyPeriodId;

		[TdfMember("DBUF")]
		public int mDailyBuffer;

		[TdfMember("DHOU")]
		public int mDailyHour;

		[TdfMember("DRET")]
		public int mDailyRetention;

		[TdfMember("MBUF")]
		public int mMonthlyBuffer;

		[TdfMember("MDAY")]
		public int mMonthlyDay;

		[TdfMember("MHOU")]
		public int mMonthlyHour;

		[TdfMember("MRET")]
		public int mMonthlyRetention;

		[TdfMember("WBUF")]
		public int mWeeklyBuffer;

		[TdfMember("WDAY")]
		public int mWeeklyDay;

		[TdfMember("WHOU")]
		public int mWeeklyHour;

		[TdfMember("WRET")]
		public int mWeeklyRetention;

	}
}
