using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct SubmitGameReportRequest
	{

		[TdfMember("FNSH")]
		public GameReportPlayerFinishedStatus mFinishedStatus;

		[TdfMember("RPRT")]
		public GameReport mGameReport;

		[TdfMember("PRVT")]
		public object? mPrivateReport;

	}
}
