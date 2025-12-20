using Blaze2SDK.Blaze.Util;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct ReplicatedGamePlayer
    {

        [TdfMember("BLOB")]
        public byte[] mCustomData;

        [TdfMember("EXID")]
        public ulong mExternalId;

        [TdfMember("GID")]
        public uint mGameId;

        [TdfMember("LOC")]
        public uint mAccountLocale;

        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mPlayerName;

        [TdfMember("NQOS")]
        public NetworkQosData mNetworkQosData;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("PATT")]
        public SortedDictionary<string, string> mPlayerAttribs;

        [TdfMember("PID")]
        public uint mPlayerId;

        [TdfMember("PNET")]
        public NetworkAddress mNetworkAddress;

        [TdfMember("SID")]
        public byte mSlotId;

        [TdfMember("SLOT")]
        public SlotType mSlotType;

        [TdfMember("STAT")]
        public PlayerState mPlayerState;

        [TdfMember("TEAM")]
        public ushort mTeamId;

        [TdfMember("TIDX")]
        public ushort mTeamIndex;

        [TdfMember("TIME")]
        public long mJoinedGameTimestamp;

        [TdfMember("UID")]
        public uint mPlayerSessionId;

    }
}
