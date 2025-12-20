using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyHostMigrationStart
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("HOST")]
        public uint mNewHostId;
        
        [TdfMember("PMIG")]
        public HostMigrationType mMigrationType;
        
        [TdfMember("SLOT")]
        public byte mNewHostSlotId;
        
    }
}
