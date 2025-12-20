using Tdf;

namespace Blaze3SDK.Blaze.GameReporting.Shooter
{
	[TdfStruct(0x422A46E1)]
	public struct GroupReport
	{

		[TdfMember("PLYR")]
		public SortedDictionary<long, EntityReport> mPlayerReports;

		[TdfMember("ATRB")]
		public SortedDictionary<string, float> mStats;

	}
}
