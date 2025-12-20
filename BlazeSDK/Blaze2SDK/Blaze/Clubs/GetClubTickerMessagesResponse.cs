using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubTickerMessagesResponse
    {
        
        [TdfMember("MSLI")]
        public List<ClubTickerMessage> mMsgList;
        
    }
}
