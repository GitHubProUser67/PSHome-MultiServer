using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyMatchmakingAsyncStatus
    {
        
        [TdfMember("ASIL")]
        public List<MatchmakingAsyncStatus> mMatchmakingAsyncStatusList;
        
        [TdfMember("MSID")]
        public uint mMatchmakingSessionId;
        
        [TdfMember("USID")]
        public uint mUserSessionId;
        
    }
}
