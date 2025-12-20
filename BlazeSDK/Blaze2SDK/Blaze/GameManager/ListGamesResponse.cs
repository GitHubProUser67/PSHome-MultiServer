using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct ListGamesResponse
    {
        
        [TdfMember("LGAM")]
        public List<ListGameData> mGames;
        
    }
}
