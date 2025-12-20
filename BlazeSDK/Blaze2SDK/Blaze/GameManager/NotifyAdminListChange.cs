using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyAdminListChange
    {
        
        [TdfMember("ALST")]
        public uint mAdminPlayerId;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("OPER")]
        public UpdateAdminListOperation mOperation;
        
        [TdfMember("UID")]
        public uint mUpdaterPlayerId;
        
    }
}
