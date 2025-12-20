using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReports
	{

		[TdfMember("QUER")]
		public GameReportQuery mGameReportQuery;

		[TdfMember("MGRR")]
		public uint mMaxGameReport;

		[TdfMember("QNAM")]
		public string mQueryName;

		[TdfMember("QVAR")]
		public List<string> mQueryVarValues;

	}
}
