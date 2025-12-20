using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct ResultNotification
	{

		[TdfMember("EROR")]
		public int mBlazeError;

		[TdfMember("DATA")]
		public object? mCustomData;

		[TdfMember("FNL")]
		public bool mFinalResult;

		[TdfMember("GHID")]
		public ulong mGameHistoryId;

		[TdfMember("GRID")]
		public ulong mGameReportingId;

	}
}
