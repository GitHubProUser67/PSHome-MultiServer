using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct ScopeValues
	{

		[TdfMember("KSVL")]
		public SortedDictionary<long, long> mKeyScopeValues;

	}
}
