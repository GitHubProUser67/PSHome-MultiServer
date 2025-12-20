using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct MatchmakingSetupContext
	{

		[TdfMember("FIT")]
		public uint mFitScore;

		[TdfMember("RSLT")]
		public MatchmakingResult mMatchmakingResult;

		[TdfMember("MAXF")]
		public uint mMaxPossibleFitScore;

		[TdfMember("MSID")]
		public uint mSessionId;

		[TdfMember("USID")]
		public uint mUserSessionId;

	}
}
