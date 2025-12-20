using Tdf;

namespace Blaze3SDK.Blaze.GameManager
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

		[TdfMember("GSRD")]
		public GameSizeRuleStatus mGameSizeRuleStatus;

		[TdfMember("GRDA")]
		public SortedDictionary<string, GenericRuleStatus> mGenericRuleStatusMap;

		[TdfMember("GEOS")]
		public GeoLocationRuleStatus mGeoLocationRuleStatus;

		[TdfMember("HBRD")]
		public HostBalanceRuleStatus mHostBalanceRuleStatus;

		[TdfMember("HVRD")]
		public HostViabilityRuleStatus mHostViabilityRuleStatus;

		[TdfMember("PSRS")]
		public PingSiteRuleStatus mPingSiteRuleStatus;

		[TdfMember("RRDA")]
		public RankRuleStatus mRankRuleStatus;

		[TdfMember("SKRS")]
		public SortedDictionary<string, SkillRuleStatus> mSkillRuleStatusMap;

		[TdfMember("TSRS")]
		public TeamSizeRuleStatus mTeamSizeRuleStatus;

		[TdfMember("UEDS")]
		public SortedDictionary<string, UEDRuleStatus> mUEDRuleStatusMap;

		[TdfMember("VGRS")]
		public VirtualGameRuleStatus mVirtualGameRuleStatus;

	}
}
