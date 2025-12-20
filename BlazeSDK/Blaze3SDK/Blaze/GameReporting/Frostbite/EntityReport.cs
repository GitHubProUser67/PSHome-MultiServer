using Tdf;

namespace Blaze3SDK.Blaze.GameReporting.Frostbite
{
	[TdfStruct(0xC951AD99)]
	public struct EntityReport
	{

		[TdfMember("STAT")]
		public SortedDictionary<string, float> mStats;

	}
}
