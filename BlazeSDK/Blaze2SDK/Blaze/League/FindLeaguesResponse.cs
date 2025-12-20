using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct FindLeaguesResponse
    {
        
        [TdfMember("LLST")]
        public List<League> mLeagues;
        
    }
}
