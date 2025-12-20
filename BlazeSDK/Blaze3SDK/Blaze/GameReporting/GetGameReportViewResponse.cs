using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReportViewResponse
	{

		[TdfMember("LGRC")]
		public List<GameReportColumn> mColumns;

		[TdfMember("ENID")]
		public List<long> mEntityIds;

	}
}
