using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetMatchmakingConfigResponse
	{

		[TdfMember("GLST")]
		public List<GenericRuleConfig> mGenericRules;

		[TdfMember("PPSR")]
		public PredefinedPingSiteRuleConfig mPingSiteRule;

		[TdfMember("RLST")]
		public List<PredefinedRuleConfig> mPredefinedRules;

	}
}
