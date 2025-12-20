using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct MatchmakingCriteriaData
    {
        
        [TdfMember("CUST")]
        public MatchmakingCustomCriteriaData mCustomRulePrefs;
        
        [TdfMember("DNF")]
        public DNFRulePrefs mDNFRulePrefs;
        
        [TdfMember("GEO")]
        public GeoLocationRuleCriteria mGeoLocationRuleCriteria;
        
        [TdfMember("GVER")]
        public int mGameProtocolVersion;
        
        [TdfMember("NAT")]
        public HostBalancingRulePrefs mHostBalancingRulePrefs;
        
        [TdfMember("PSR")]
        public PingSiteRulePrefs mPingSiteRulePrefs;
        
        [TdfMember("RANK")]
        public RankedGameRulePrefs mRankedGameRulePrefs;
        
        [TdfMember("RLST")]
        public List<GenericRulePrefs> mGenericRulePrefsList;
        
        [TdfMember("RSZR")]
        public RosterSizeRulePrefs mRosterSizeRulePrefs;
        
        [TdfMember("SIZE")]
        public GameSizeRulePrefs mGameSizeRulePrefs;
        
        [TdfMember("SKLZ")]
        public List<SkillRulePrefs> mSkillRulePrefsList;
        
        [TdfMember("TEAM")]
        public TeamSizeRulePrefs mTeamSizeRulePrefs;
        
        [TdfMember("VIAB")]
        public HostViabilityRulePrefs mHostViabilityRulePrefs;
        
    }
}
