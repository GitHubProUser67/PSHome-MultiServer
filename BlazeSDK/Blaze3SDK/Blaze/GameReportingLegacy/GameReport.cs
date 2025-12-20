using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReport
	{

		[TdfMember("ATRB")]
		public SortedDictionary<string, string> mAttributeMap;

		[TdfMember("FNSH")]
		public bool mFinished;

		[TdfMember("GRID")]
		public ulong mGameReportingId;

		[TdfMember("GTYP")]
		public uint mGameTypeId;

		[TdfMember("RPRT")]
		public SortedDictionary<long, Report> mPlayerReportMap;

		[TdfMember("PRCS")]
		public bool mProcess;

		[TdfMember("RTM")]
		public SortedDictionary<string, ReportType> mReportTypeMap;

        [TdfStruct]
        public struct Report
        {

            [TdfMember("RPRT")]
            public SortedDictionary<string, string> mAttributeMap;

        }

        [TdfStruct]
        public struct ReportType
        {

            [TdfMember("RPMP")]
            public SortedDictionary<long, Report> mReportMap;

        }

    }
}
