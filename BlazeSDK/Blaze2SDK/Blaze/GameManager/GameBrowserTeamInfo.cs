using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameBrowserTeamInfo
    {
        
        [TdfMember("TCAP")]
        public ushort mTeamCapacity;
        
        [TdfMember("TID")]
        public ushort mTeamId;
        
        [TdfMember("TSZE")]
        public ushort mTeamSize;
        
    }
}
