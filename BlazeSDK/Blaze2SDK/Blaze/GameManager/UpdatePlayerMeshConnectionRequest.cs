using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct UpdatePlayerMeshConnectionRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("TARG")]
        public List<PlayerConnectionStatus> mPlayersMeshConnection;
        
    }
}
