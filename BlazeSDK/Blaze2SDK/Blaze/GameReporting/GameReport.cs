using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReport
    {

        [TdfStruct]
        public struct Report
        {

            /// <summary>
            /// Max Key String Length: 32
            /// Max Value String Length: 256
            /// </summary>
            [TdfMember("RPRT")]
            public SortedDictionary<string, string> mAttributeMap;

        }

        [TdfStruct]
        public struct ReportType
        {

            [TdfMember("RPMP")]
            public SortedDictionary<uint, Report> mReportMap;

        }

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATRB")]
        public SortedDictionary<string, string> mAttributeMap;

        [TdfMember("FNSH")]
        public bool mFinished;

        [TdfMember("GRID")]
        public uint mGameReportingId;

        [TdfMember("GTYP")]
        public uint mGameTypeId;

        [TdfMember("PRCS")]
        public bool mProcess;

        [TdfMember("RPRT")]
        public SortedDictionary<uint, Report> mPlayerReportMap;

        /// <summary>
        /// Max Key String Length: 64
        /// </summary>
        [TdfMember("RTM")]
        public SortedDictionary<string, ReportType> mReportTypeMap;

    }
}
