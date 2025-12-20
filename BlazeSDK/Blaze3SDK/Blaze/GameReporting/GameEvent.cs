using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameEvent
	{

		[TdfMember("DATA")]
		public object? mGameEventData;

		[TdfMember("GMET")]
		public uint mGameEventType;

	}
}
