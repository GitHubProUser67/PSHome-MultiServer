using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SetGameSettingsRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("GSET")]
        public GameSettings mGameSettings;
        
    }
}
