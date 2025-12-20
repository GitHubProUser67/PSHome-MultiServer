using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct MatchmakingCriteriaData
	{

		[TdfMember("CUST")]
		public MatchmakingCustomCriteriaData mCustomRulePrefs;

		[TdfMember("DNF")]
		public DNFRulePrefs mDNFRulePrefs;

		[TdfMember("GNAM")]
		public GameNameRuleCriteria mGameNameRuleCriteria;

		[TdfMember("SIZE")]
		public GameSizeRulePrefs mGameSizeRulePrefs;

		[TdfMember("RLST")]
		public List<GenericRulePrefs> mGenericRulePrefsList;

		[TdfMember("GEO")]
		public GeoLocationRuleCriteria mGeoLocationRuleCriteria;

		[TdfMember("NAT")]
		public HostBalancingRulePrefs mHostBalancingRulePrefs;

		[TdfMember("VIAB")]
		public HostViabilityRulePrefs mHostViabilityRulePrefs;

		[TdfMember("PSR")]
		public PingSiteRulePrefs mPingSiteRulePrefs;

		[TdfMember("RANK")]
		public RankedGameRulePrefs mRankedGameRulePrefs;

		[TdfMember("RSZR")]
		public RosterSizeRulePrefs mRosterSizeRulePrefs;

		[TdfMember("SKLZ")]
		public List<SkillRulePrefs> mSkillRulePrefsList;

		[TdfMember("TEAM")]
		public TeamSizeRulePrefs mTeamSizeRulePrefs;

		[TdfMember("UED")]
		public SortedDictionary<string, UEDRuleCriteria> mUEDRuleCriteriaMap;

		[TdfMember("VIRT")]
		public VirtualGameRulePrefs mVirtualGameRulePrefs;

	}
}
