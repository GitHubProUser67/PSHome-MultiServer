using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SetPlayerCustomDataRequest
    {
        
        [TdfMember("CDAT")]
        public byte[] mCustomData;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
    }
}
