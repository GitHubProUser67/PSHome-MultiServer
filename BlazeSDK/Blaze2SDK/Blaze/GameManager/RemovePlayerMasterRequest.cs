using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RemovePlayerMasterRequest
    {
        
        [TdfMember("CNTX")]
        public ushort mPlayerRemovedTitleContext;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("NID")]
        public long mNucleusId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
        [TdfMember("REAS")]
        public PlayerRemovedReason mPlayerRemovedReason;
        
    }
}
