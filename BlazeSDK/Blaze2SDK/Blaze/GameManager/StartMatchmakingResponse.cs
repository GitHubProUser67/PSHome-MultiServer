using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct StartMatchmakingResponse
    {
        
        [TdfMember("MSID")]
        public uint mSessionId;
        
    }
}
