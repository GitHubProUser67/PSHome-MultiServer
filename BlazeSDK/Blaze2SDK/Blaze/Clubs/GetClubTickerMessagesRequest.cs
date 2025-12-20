using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubTickerMessagesRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("TSTM")]
        public uint mOldestTimestamp;
        
    }
}
