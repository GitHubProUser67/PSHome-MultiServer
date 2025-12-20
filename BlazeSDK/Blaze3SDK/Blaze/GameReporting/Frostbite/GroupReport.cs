using Tdf;

namespace Blaze3SDK.Blaze.GameReporting.Frostbite
{
	[TdfStruct(0x71F381B1)]
	public struct GroupReport
	{

		[TdfMember("PLYR")]
		public SortedDictionary<long, EntityReport> mPlayerReports;

		[TdfMember("ATRB")]
		public SortedDictionary<string, float> mStats;

	}
}
