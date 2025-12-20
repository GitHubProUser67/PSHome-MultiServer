using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameSettingsChange
    {
        
        [TdfMember("ATTR")]
        public GameSettings mGameSettings;
        
        [TdfMember("GID")]
        public uint mGameId;
        
    }
}
