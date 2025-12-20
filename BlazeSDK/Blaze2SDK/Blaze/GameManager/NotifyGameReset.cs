using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameReset
    {
        
        [TdfMember("DATA")]
        public ReplicatedGameData mGameData;
        
    }
}
