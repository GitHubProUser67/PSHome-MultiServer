using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct SendMessageResponse
    {
        
        [TdfMember("MGID")]
        public uint mMessageId;
        
    }
}
