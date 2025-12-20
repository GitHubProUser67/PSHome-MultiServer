using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RemovePlayerRequest
    {
        
        [TdfMember("BTPL")]
        public ulong mGroupId;
        
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
