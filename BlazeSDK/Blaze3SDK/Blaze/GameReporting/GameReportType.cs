using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportType
	{

		[TdfMember("GTNA")]
		public string mGameTypeName;

		[TdfMember("HIST")]
		public List<TableData> mHistoryTables;

	}
}
