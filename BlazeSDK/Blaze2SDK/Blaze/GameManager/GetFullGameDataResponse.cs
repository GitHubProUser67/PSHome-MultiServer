using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GetFullGameDataResponse
    {
        
        [TdfMember("LGAM")]
        public List<FullGameData> mGames;
        
    }
}
