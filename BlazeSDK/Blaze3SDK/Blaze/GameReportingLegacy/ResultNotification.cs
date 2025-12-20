using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct ResultNotification
	{

		[TdfMember("EROR")]
		public int mBlazeError;

		[TdfMember("FNL")]
		public bool mFinalResult;

		[TdfMember("GRID")]
		public ulong mGameReportingId;

	}
}
