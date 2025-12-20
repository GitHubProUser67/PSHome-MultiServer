using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct ClientMessage
    {
        
        /// <summary>
        /// Max Value String Length: 1024
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<uint, string> mAttrMap;
        
        [TdfMember("FLAG")]
        public MessageFlags mFlags;
        
        [TdfMember("STAT")]
        public uint mStatus;
        
        [TdfMember("TAG")]
        public uint mTag;
        
        [TdfMember("TARG")]
        public ulong mTarget;
        
        [TdfMember("TYPE")]
        public uint mType;
        
    }
}
