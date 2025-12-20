using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct DateRange
	{

		[TdfMember("END")]
		public uint mEnd;

		[TdfMember("STRT")]
		public uint mStart;

	}
}
