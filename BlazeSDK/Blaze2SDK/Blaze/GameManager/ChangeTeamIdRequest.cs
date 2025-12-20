using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct ChangeTeamIdRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("NTID")]
        public ushort mNewTeamId;
        
        [TdfMember("OTID")]
        public ushort mOldTeamId;
        
        [TdfMember("TIDX")]
        public ushort mTeamIndex;
        
    }
}
