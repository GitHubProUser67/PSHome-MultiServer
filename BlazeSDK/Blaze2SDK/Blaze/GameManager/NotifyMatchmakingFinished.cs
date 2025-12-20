using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyMatchmakingFinished
    {
        
        [TdfMember("FIT")]
        public uint mFitScore;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("MAXF")]
        public uint mMaxPossibleFitScore;
        
        [TdfMember("MSID")]
        public uint mSessionId;
        
        [TdfMember("RSLT")]
        public MatchmakingResult mMatchmakingResult;
        
        [TdfMember("USID")]
        public uint mUserSessionId;
        
    }
}
