using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NumOfPlayerSessionsResponse
    {
        
        [TdfMember("NOMM")]
        public uint mNumOfPlayerSessions;
        
    }
}
