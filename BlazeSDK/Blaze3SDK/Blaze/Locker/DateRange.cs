using Tdf;

namespace Blaze3SDK.Blaze.Locker
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
