using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct IndirectMatchmakingSetupContext
	{

		[TdfMember("FIT")]
		public uint mFitScore;

		[TdfMember("RSLT")]
		public MatchmakingResult mMatchmakingResult;

		[TdfMember("MAXF")]
		public uint mMaxPossibleFitScore;

		[TdfMember("RPVC")]
		public bool mRequiresClientVersionCheck;

		[TdfMember("MSID")]
		public uint mSessionId;

		[TdfMember("GRID")]
		public BlazeObjectId mUserGroupId;

		[TdfMember("USID")]
		public uint mUserSessionId;

	}
}
