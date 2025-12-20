using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct CountMessagesRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("MSTY")]
        public MessageType mMessageType;
        
    }
}
