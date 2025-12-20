using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameEvent
	{

		[TdfMember("EATR")]
		public SortedDictionary<string, string> mAttributeMap;

		[TdfMember("GMET")]
		public uint mGameEventType;

	}
}
