using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct CreateGameResponse
    {
        
        [TdfMember("DATA")]
        public ReplicatedGameData mGameData;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("HID")]
        public uint mHostId;
        
        [TdfMember("PROS")]
        public List<ReplicatedGamePlayer> mGameRoster;
        
    }
}
