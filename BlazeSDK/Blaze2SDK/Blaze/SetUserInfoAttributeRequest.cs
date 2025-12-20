using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct SetUserInfoAttributeRequest
    {
        
        [TdfMember("ATTV")]
        public ulong mAttribute;
        
        [TdfMember("MASK")]
        public ulong mMask;
        
        [TdfMember("ULST")]
        public List<ulong> mBlazeObjectIdList;
        
    }
}
