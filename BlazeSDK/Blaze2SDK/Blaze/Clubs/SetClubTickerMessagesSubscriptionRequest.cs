using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct SetClubTickerMessagesSubscriptionRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("ISSU")]
        public bool mIsSubscribed;
        
    }
}
