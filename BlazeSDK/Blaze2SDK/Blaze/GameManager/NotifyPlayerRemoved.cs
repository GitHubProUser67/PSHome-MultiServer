using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyPlayerRemoved
    {
        
        [TdfMember("CNTX")]
        public ushort mPlayerRemovedTitleContext;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
        [TdfMember("REAS")]
        public PlayerRemovedReason mPlayerRemovedReason;
        
    }
}
