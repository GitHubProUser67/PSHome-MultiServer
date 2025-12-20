using Tdf;

namespace Blaze3SDK.Blaze.GameReporting.Shooter
{
	[TdfStruct(0xFFF38E69)]
	public struct EntityReport
	{

		[TdfMember("STAT")]
		public SortedDictionary<string, float> mStats;

	}
}
