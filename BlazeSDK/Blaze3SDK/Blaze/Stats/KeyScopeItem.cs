using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct KeyScopeItem
	{

		[TdfMember("AGKY")]
		public long mAggregateKeyValue;

		[TdfMember("ENAG")]
		public bool mEnableAggregation;

		[TdfMember("KSVL")]
		public SortedDictionary<long, long> mKeyScopeValues;

	}
}
