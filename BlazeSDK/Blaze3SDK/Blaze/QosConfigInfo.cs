using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct QosConfigInfo
	{

		[TdfMember("BWPS")]
		public QosPingSiteInfo mBandwidthPingSiteInfo;

		[TdfMember("LNP")]
		public ushort mNumLatencyProbes;

		[TdfMember("LTPS")]
		public SortedDictionary<string, QosPingSiteInfo> mPingSiteInfoByAliasMap;

		[TdfMember("SVID")]
		public uint mServiceId;

	}
}
