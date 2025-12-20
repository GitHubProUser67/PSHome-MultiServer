using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct AddressRemapEntry
    {
        
        [TdfMember("DPRT")]
        public ushort mDstPort;
        
        [TdfMember("MASK")]
        public uint mNetMask;
        
        [TdfMember("SID")]
        public uint mServiceId;
        
        [TdfMember("SIP")]
        public uint mSrcIp;
        
        [TdfMember("SPRT")]
        public ushort mSrcPort;
        
    }
}
