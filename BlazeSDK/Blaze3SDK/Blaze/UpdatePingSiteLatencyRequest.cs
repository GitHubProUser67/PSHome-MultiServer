using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UpdatePingSiteLatencyRequest
	{

		[TdfMember("NLMP")]
		public SortedDictionary<string, int> mPingSiteLatencyByAliasMap;

	}
}
