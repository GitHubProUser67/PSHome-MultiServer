using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetStatsResponse
	{

		[TdfMember("KSSV")]
		public SortedDictionary<string, StatValues> mKeyScopeStatsValueMap;

	}
}
