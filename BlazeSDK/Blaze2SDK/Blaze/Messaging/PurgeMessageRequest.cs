using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct PurgeMessageRequest
    {

        [TdfMember("FLAG")]
        public MatchFlags mFlags;

        [TdfMember("MGID")]
        public uint mMessageId;

        [TdfMember("SMSK")]
        public uint mStatusMask;

        [TdfMember("SRCE")]
        public ulong mSource;

        [TdfMember("STAT")]
        public uint mStatus;

        [TdfMember("TYPE")]
        public uint mType;

    }
}
