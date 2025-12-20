using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct PlayerConnectionStatus
    {
        
        [TdfMember("PID")]
        public uint mTargetPlayer;
        
        [TdfMember("STAT")]
        public PlayerNetConnectionStatus mPlayerNetConnectionStatus;
        
    }
}
