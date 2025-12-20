using Blaze2SDK.Blaze.Util;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct ReplicatedGameData
    {

        [TdfMember("ADMN")]
        public List<uint> mAdminPlayerList;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mGameAttribs;

        [TdfMember("CAP")]
        public List<ushort> mSlotCapacities;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteriaMap;

        [TdfMember("GID")]
        public uint mGameId;

        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(32)]
        public string mGameName;

        [TdfMember("GPVH")]
        public ulong mGameProtocolVersionHash;

        [TdfMember("GSET")]
        public GameSettings mGameSettings;

        [TdfMember("GSID")]
        public uint mGameReportingId;

        [TdfMember("GSTA")]
        public GameState mGameState;

        [TdfMember("GVER")]
        public int mGameProtocolVersion;

        [TdfMember("HNET")]
        public List<NetworkAddress> mHostNetworkAddressList;

        [TdfMember("HSES")]
        public uint mTopologyHostSessionId;

        [TdfMember("IGNO")]
        public bool mIgnoreEntryCriteriaWithInvite;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("MATR")]
        public SortedDictionary<string, string> mMeshAttribs;

        [TdfMember("MCAP")]
        public ushort mMaxPlayerCapacity;

        [TdfMember("NQOS")]
        public NetworkQosData mNetworkQosData;

        [TdfMember("NTOP")]
        public GameNetworkTopology mNetworkTopology;

        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("PGID")]
        [StringLength(36)]
        public string mPersistedGameId;

        [TdfMember("PGSR")]
        public byte[] mPersistedGameIdSecret;

        [TdfMember("PHST")]
        public HostInfo mPlatformHostInfo;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PSAS")]
        [StringLength(64)]
        public string mPingSiteAlias;

        [TdfMember("QCAP")]
        public ushort mQueueCapacity;

        [TdfMember("SEED")]
        public uint mSharedSeed;

        [TdfMember("TCAP")]
        public List<TeamCapacity> mTeamCapacities;

        [TdfMember("THST")]
        public HostInfo mTopologyHostInfo;

        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("UUID")]
        [StringLength(36)]
        public string mUUID;

        [TdfMember("VOIP")]
        public VoipTopology mVoipNetwork;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VSTR")]
        [StringLength(64)]
        public string mGameProtocolVersionString;

        [TdfMember("XNNC")]
        public byte[] mXnetNonce;

        [TdfMember("XSES")]
        public byte[] mXnetSession;

    }
}
