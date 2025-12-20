using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportQuery
	{

		[TdfMember("COLS")]
		public List<GameReportColumnKey> mColumnKeyList;

		[TdfMember("FILT")]
		public List<GameReportFilter> mFilterList;

		[TdfMember("MGRR")]
		public uint mMaxGameReport;

		[TdfMember("QNAM")]
		public string mName;

		[TdfMember("GTYP")]
		public string mTypeName;

	}
}
