using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct UpdatePlaygroupSessionRequest
    {
        
        [TdfMember("PGID")]
        public uint mId;
        
        [TdfMember("XNNC")]
        public byte[] mXnetNonce;
        
        [TdfMember("XSES")]
        public byte[] mXnetSession;
        
    }
}
