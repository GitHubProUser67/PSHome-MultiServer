using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SetPlayerTeamRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
        [TdfMember("TEAM")]
        public ushort mTeamId;
        
        [TdfMember("TIDX")]
        public ushort mTeamIndex;
        
    }
}
