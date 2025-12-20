using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyHostMigrationFinished
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
    }
}
