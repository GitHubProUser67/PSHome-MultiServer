using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct MigrateHostRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("HOST")]
        public uint mNewHostPlayer;
        
    }
}
