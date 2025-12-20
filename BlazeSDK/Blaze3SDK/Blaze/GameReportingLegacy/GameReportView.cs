using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportView
	{

		[TdfMember("LGRC")]
		public List<GameReportColumn> mColumns;

		[TdfMember("ENID")]
		public List<long> mEntityIds;

	}
}
