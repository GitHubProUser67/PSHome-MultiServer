using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct PredefinedPingSiteRuleConfig
	{

		[TdfMember("POSV")]
		public List<string> mPossibleValues;

		[TdfMember("PDRC")]
		public PredefinedRuleConfig mPredefinedRuleConfig;

	}
}
