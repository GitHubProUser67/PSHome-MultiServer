using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct CancelMatchmakingRequest
    {
        
        [TdfMember("MSID")]
        public uint mMatchmakingSessionId;
        
    }
}
