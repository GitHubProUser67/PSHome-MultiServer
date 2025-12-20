using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportsList
	{

		[TdfMember("GMRS")]
		public List<GameReport> mGameReportList;

	}
}
