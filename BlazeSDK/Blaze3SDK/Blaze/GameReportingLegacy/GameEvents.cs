using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameEvents
	{

		[TdfMember("GMES")]
		public List<GameEvent> mGameEvents;

	}
}
