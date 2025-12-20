using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct PurgeMessageResponse
    {
        
        [TdfMember("MCNT")]
        public uint mCount;
        
    }
}
