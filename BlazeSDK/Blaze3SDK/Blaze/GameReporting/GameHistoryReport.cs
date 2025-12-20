using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameHistoryReport
	{

		[TdfMember("GHID")]
		public ulong mGameHistoryId;

		[TdfMember("GRID")]
		public ulong mGameReportingId;

		[TdfMember("GTYP")]
		public string mGameTypeName;

		[TdfMember("TRM")]
		public SortedDictionary<string, TableRows> mTableRowMap;

		[TdfMember("TIME")]
		public long mTimestamp;

        [TdfStruct]
        public struct TableRow
        {

            [TdfMember("TROW")]
            public SortedDictionary<string, string> mAttributeMap;

        }

        [TdfStruct]
        public struct TableRows
        {

            [TdfMember("RLIS")]
            public List<TableRow> mTableRowList;

        }

    }
}
