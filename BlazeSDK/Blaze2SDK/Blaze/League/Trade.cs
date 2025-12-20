using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct Trade
    {
        
        [TdfMember("CRTM")]
        public uint mCreationTime;
        
        [TdfMember("FORM")]
        public LeagueUser mOriginator;
        
        [TdfMember("FORP")]
        public uint mOriginatorPlayerId;
        
        [TdfMember("LATP")]
        public uint mRecipientPlayerId;
        
        [TdfMember("LATT")]
        public LeagueUser mRecipient;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("TDID")]
        public uint mTradeId;
        
        public enum TradeOp : int
        {
            TRADE_ACCEPT = 0x0,
            TRADE_REJECT = 0x1,
            TRADE_REVOKE = 0x2,
        }
        
        public enum TradeType : int
        {
            LEAGUE_TRADES_NONE = 0x0,
            LEAGUE_TRADES_SIMPLE = 0x1,
            LEAGUE_TRADES_RESTRICTED = 0x2,
        }
        
    }
}
