using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportsList
	{

		[TdfMember("GMRS")]
		public List<GameHistoryReport> mGameReportList;

	}
}
