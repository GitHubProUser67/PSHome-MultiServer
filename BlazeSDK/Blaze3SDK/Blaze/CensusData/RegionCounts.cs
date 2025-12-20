using Tdf;

namespace Blaze3SDK.Blaze.CensusData
{
	[TdfStruct(0x2D9E9E89)]
	public struct RegionCounts
	{

		[TdfMember("CNOU")]
		public SortedDictionary<string, uint> mNumOfUsersByRegion;

	}
}
