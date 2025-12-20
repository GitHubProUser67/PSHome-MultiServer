using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct ProcessTradeRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("STAT")]
        public TradeOp mOperation;
        
        [TdfMember("TDID")]
        public uint mTradeId;
        
    }
}
