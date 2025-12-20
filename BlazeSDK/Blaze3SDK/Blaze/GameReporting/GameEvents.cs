using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameEvents
	{

		[TdfMember("PROC")]
		public string mGameEventProcessorName;

		[TdfMember("GMES")]
		public List<GameEvent> mGameEvents;

	}
}
