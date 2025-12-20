using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetTradesResponse
    {
        
        [TdfMember("TRLI")]
        public List<Trade> mTrades;
        
    }
}
