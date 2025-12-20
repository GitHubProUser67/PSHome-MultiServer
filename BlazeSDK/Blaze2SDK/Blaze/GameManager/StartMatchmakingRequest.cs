using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct StartMatchmakingRequest
    {

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mGameAttribs;

        [TdfMember("BTPL")]
        public ulong mGroupId;

        [TdfMember("CRIT")]
        public MatchmakingCriteriaData mCriteriaData;

        [TdfMember("DUR")]
        public uint mSessionDurationMS;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("ECRI")]
        public SortedDictionary<string, string> mEntryCriteriaMap;

        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(32)]
        public string mGameName;

        [TdfMember("GSET")]
        public GameSettings mGameSettings;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GVER")]
        [StringLength(64)]
        public string mGameProtocolVersionString;

        [TdfMember("IGNO")]
        public bool mIgnoreEntryCriteriaWithInvite;

        [TdfMember("MODE")]
        public MatchmakingSessionMode mSessionMode;

        [TdfMember("NTOP")]
        public GameNetworkTopology mNetworkTopology;

        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("PATT")]
        public SortedDictionary<string, string> mPlayerAttribs;

        [TdfMember("PLST")]
        public List<uint> mPlayerIdList;

        [TdfMember("PMAX")]
        public ushort mMaxPlayerCapacity;

        [TdfMember("PNET")]
        public NetworkAddress mPlayerNetworkAddress;

        [TdfMember("QCAP")]
        public ushort mQueueCapacity;

        [TdfMember("SIDL")]
        public List<uint> mSessionIdList;

        [TdfMember("VOIP")]
        public VoipTopology mVoipNetwork;

    }
}
