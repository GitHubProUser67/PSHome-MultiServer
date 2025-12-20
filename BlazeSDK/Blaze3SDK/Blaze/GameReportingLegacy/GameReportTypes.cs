using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportTypes
	{

		[TdfMember("GRTS")]
		public List<GameReportType> mGameReportTypes;

	}
}
