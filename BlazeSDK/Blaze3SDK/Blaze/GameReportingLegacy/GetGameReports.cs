using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GetGameReports
	{

		[TdfMember("MGRR")]
		public uint mMaxGameReport;

		[TdfMember("QNAM")]
		public string mQueryName;

		[TdfMember("QVAR")]
		public List<string> mQueryVarValues;

	}
}
