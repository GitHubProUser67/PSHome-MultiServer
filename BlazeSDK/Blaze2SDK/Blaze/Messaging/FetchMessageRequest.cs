using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct FetchMessageRequest
    {
        
        [TdfMember("FLAG")]
        public MatchFlags mFlags;
        
        [TdfMember("MGID")]
        public uint mMessageId;
        
        [TdfMember("PIDX")]
        public uint mPageIndex;
        
        [TdfMember("PSIZ")]
        public uint mPageSize;
        
        [TdfMember("SMSK")]
        public uint mStatusMask;
        
        [TdfMember("SORT")]
        public MessageOrder mOrderBy;
        
        [TdfMember("SRCE")]
        public ulong mSource;
        
        [TdfMember("STAT")]
        public uint mStatus;
        
        [TdfMember("TYPE")]
        public uint mType;
        
    }
}
