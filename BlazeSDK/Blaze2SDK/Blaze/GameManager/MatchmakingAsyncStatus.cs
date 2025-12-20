using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct MatchmakingAsyncStatus
    {
        
        [TdfMember("CGS")]
        public CreateGameStatus mCreateGameStatus;
        
        [TdfMember("CUST")]
        public MatchmakingCustomAsyncStatus mCustomAsynStatus;
        
        [TdfMember("DNFS")]
        public DNFRuleStatus mDNFRuleStatus;
        
        [TdfMember("FGS")]
        public FindGameStatus mFindGameStatus;
        
        [TdfMember("GEOS")]
        public GeoLocationRuleStatus mGeoLocationRuleStatus;
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("GRDA")]
        public SortedDictionary<string, GenericRuleStatus> mGenericRuleStatusMap;
        
        [TdfMember("GSRD")]
        public GameSizeRuleStatus mGameSizeRuleStatus;
        
        [TdfMember("HBRD")]
        public HostBalanceRuleStatus mHostBalanceRuleStatus;
        
        [TdfMember("HVRD")]
        public HostViabilityRuleStatus mHostViabilityRuleStatus;
        
        [TdfMember("PSRS")]
        public PingSiteRuleStatus mPingSiteRuleStatus;
        
        [TdfMember("RRDA")]
        public RankRuleStatus mRankRuleStatus;
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("SKRS")]
        public SortedDictionary<string, SkillRuleStatus> mSkillRuleStatusMap;
        
        [TdfMember("TSRS")]
        public TeamSizeRuleStatus mTeamSizeRuleStatus;
        
    }
}
