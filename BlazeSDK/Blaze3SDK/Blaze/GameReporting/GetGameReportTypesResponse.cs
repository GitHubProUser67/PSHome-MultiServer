using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReportTypesResponse
	{

		[TdfMember("GRTS")]
		public List<GameReportType> mGameReportTypes;

	}
}
