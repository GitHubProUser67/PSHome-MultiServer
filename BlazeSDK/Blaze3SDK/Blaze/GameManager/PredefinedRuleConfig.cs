using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct PredefinedRuleConfig
	{

		[TdfMember("RNME")]
		public string mRuleName;

		[TdfMember("THLS")]
		public List<string> mThresholdNames;

		[TdfMember("WGHT")]
		public uint mWeight;

	}
}
