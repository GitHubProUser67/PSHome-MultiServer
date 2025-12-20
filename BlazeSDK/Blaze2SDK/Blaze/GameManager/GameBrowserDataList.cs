using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameBrowserDataList
    {
        
        [TdfMember("GDAT")]
        public List<GameBrowserGameData> mGameData;
        
    }
}
