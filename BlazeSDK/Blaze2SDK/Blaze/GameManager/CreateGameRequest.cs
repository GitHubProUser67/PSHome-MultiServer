using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct CreateGameRequest
    {
        
        [TdfMember("ADMN")]
        public List<uint> mAdminPlayerList;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mGameAttribs;
        
        [TdfMember("BTPL")]
        public ulong mGroupId;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 64
        /// </summary>
        [TdfMember("CRIT")]
        public SortedDictionary<string, string> mEntryCriteriaMap;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GCTR")]
        [StringLength(64)]
        public string mGamePingSiteAlias;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(32)]
        public string mGameName;
        
        [TdfMember("GSET")]
        public GameSettings mGameSettings;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("GURL")]
        [StringLength(255)]
        public string mGameStatusUrl;
        
        [TdfMember("GVER")]
        public int mGameProtocolVersion;
        
        [TdfMember("HNET")]
        public List<NetworkAddress> mHostNetworkAddressList;
        
        [TdfMember("IGNO")]
        public bool mIgnoreEntryCriteriaWithInvite;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("MATR")]
        public SortedDictionary<string, string> mMeshAttribs;
        
        [TdfMember("NTOP")]
        public GameNetworkTopology mNetworkTopology;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("PATT")]
        public SortedDictionary<string, string> mHostPlayerAttribs;
        
        [TdfMember("PCAP")]
        public List<ushort> mSlotCapacities;
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("PGID")]
        [StringLength(36)]
        public string mPersistedGameId;
        
        [TdfMember("PGSC")]
        public byte[] mPersistedGameIdSecret;
        
        [TdfMember("PMAX")]
        public ushort mMaxPlayerCapacity;
        
        [TdfMember("QCAP")]
        public ushort mQueueCapacity;
        
        [TdfMember("RGID")]
        public uint mReservedDynamicDSGameId;
        
        [TdfMember("SEAT")]
        public List<uint> mReservedPlayerSeats;
        
        [TdfMember("SIDL")]
        public List<uint> mSessionIdList;
        
        [TdfMember("SLOT")]
        public SlotType mJoiningSlotType;
        
        [TdfMember("TCAP")]
        public List<TeamCapacity> mTeamCapacities;
        
        [TdfMember("TEAM")]
        public ushort mJoiningTeamId;
        
        [TdfMember("TIDX")]
        public ushort mJoiningTeamIndex;
        
        [TdfMember("VOIP")]
        public VoipTopology mVoipNetwork;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VSTR")]
        [StringLength(64)]
        public string mGameProtocolVersionString;
        
    }
}
