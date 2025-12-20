using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct ProposeTradeRequest
    {
        
        [TdfMember("FORP")]
        public uint mOriginatorPlayerId;
        
        [TdfMember("LATP")]
        public uint mRecipientPlayerId;
        
        [TdfMember("LATT")]
        public uint mRecipientId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
