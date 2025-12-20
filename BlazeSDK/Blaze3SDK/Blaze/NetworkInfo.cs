using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct NetworkInfo
	{

		[TdfMember("ADDR")]
		public NetworkAddress mAddress;

		[TdfMember("NLMP")]
		public SortedDictionary<string, int> mPingSiteLatencyByAliasMap;

		[TdfMember("NQOS")]
		public Util.NetworkQosData mQosData;

	}
}
