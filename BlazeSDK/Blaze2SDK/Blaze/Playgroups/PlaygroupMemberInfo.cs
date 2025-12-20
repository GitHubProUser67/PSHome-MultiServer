using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct PlaygroupMemberInfo
    {

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mMemberAttributes;

        [TdfMember("JTIM")]
        public uint mJoinTime;

        [TdfMember("PERM")]
        public MemberPermissions mPermissions;

        [TdfMember("PNET")]
        public NetworkAddress mNetworkAddress;

        [TdfMember("SID")]
        public byte mSlotId;

        [TdfMember("USER")]
        public UserIdentification mUser;

    }
}
