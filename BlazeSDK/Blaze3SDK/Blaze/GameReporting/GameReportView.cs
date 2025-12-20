using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportView
	{

		[TdfMember("LGRC")]
		public List<GameReportColumn> mColumns;

		[TdfMember("FILT")]
		public List<GameReportFilter> mFilterList;

		[TdfMember("MAXG")]
		public uint mMaxGames;

		[TdfMember("RTYP")]
		public string mRowTypeName;

		[TdfMember("INFO")]
		public GameReportViewInfo mViewInfo;

	}
}
