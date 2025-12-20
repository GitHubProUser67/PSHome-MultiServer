using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameSizeRulePrefs
	{

		[TdfMember("PCNT")]
		public ushort mDesiredPlayerCount;

		[TdfMember("ISSG")]
		public byte mIsSingleGroupMatch;

		[TdfMember("PCAP")]
		public ushort mMaxPlayerCapacity;

		[TdfMember("THLD")]
		public string mMinFitThresholdName;

		[TdfMember("PMIN")]
		public ushort mMinPlayerCount;

	}
}
