using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReportColumnValuesResponse
	{

		[TdfMember("LGRC")]
		public List<GameReportColumnValues> mColumnValues;

		[TdfMember("ENID")]
		public List<long> mEntityIds;

	}
}
