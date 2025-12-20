using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NumOfMatchmakingResponse
    {
        
        [TdfMember("NOMM")]
        public uint mNumOfMatchmakingSessions;
        
    }
}
