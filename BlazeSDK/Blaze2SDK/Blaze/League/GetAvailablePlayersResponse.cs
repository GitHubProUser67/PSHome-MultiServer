using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetAvailablePlayersResponse
    {
        
        [TdfMember("MSLR")]
        public SortedDictionary<uint, AvailablePlayer> mPlayers;
        
    }
}
