using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct XboxClientAddress
    {
        
        [TdfMember("XDDR")]
        public byte[] XnAddr;
        
        [TdfMember("XUID")]
        public ulong Xuid;
        
    }
}
