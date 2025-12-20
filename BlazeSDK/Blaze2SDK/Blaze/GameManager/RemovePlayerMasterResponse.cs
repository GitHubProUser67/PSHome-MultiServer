using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RemovePlayerMasterResponse
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("GPLY")]
        public ReplicatedGamePlayer player;
        
    }
}
