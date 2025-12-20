using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct DynamicConfig
    {
        
        [TdfMember("AMAX")]
        public uint mMessageAttributeLimit;
        
    }
}
