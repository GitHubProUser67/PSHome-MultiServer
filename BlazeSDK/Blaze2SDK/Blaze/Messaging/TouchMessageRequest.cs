using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct TouchMessageRequest
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

        [TdfMember("TARG")]
        public ulong mTarget;

        [TdfMember("TMSK")]
        public uint mTouchStatusMask;

        [TdfMember("TSTA")]
        public uint mTouchStatus;

        [TdfMember("TYPE")]
        public uint mType;

    }
}
