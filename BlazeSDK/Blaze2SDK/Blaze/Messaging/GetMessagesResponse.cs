using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct GetMessagesResponse
    {
        
        [TdfMember("MSLT")]
        public List<ServerMessage> mMessages;
        
    }
}
