using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameBrowserGameData
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
        
        [TdfMember("FIT")]
        public uint mFitScore;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(32)]
        public string mGameName;
        
        [TdfMember("GSET")]
        public GameSettings mGameSettings;
        
        [TdfMember("GSTA")]
        public GameState mGameState;
        
        [TdfMember("GVER")]
        public int mGameProtocolVersion;
        
        [TdfMember("HNET")]
        public List<NetworkAddress> mHostNetworkAddressList;
        
        [TdfMember("HOST")]
        public uint mHostId;
        
        [TdfMember("PCNT")]
        public List<ushort> mPlayerCounts;
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("PSID")]
        [StringLength(36)]
        public string mPersistedGameId;
        
        [TdfMember("QCAP")]
        public ushort mQueueCapacity;
        
        [TdfMember("QCNT")]
        public ushort mQueueCount;
        
        [TdfMember("ROST")]
        public List<GameBrowserPlayerData> mGameRoster;
        
        [TdfMember("SID")]
        public ulong mExternalSessionId;
        
        [TdfMember("TINF")]
        public List<GameBrowserTeamInfo> mGameBrowserTeamInfoVector;
        
        [TdfMember("VOIP")]
        public VoipTopology mVoipTopology;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VSTR")]
        [StringLength(64)]
        public string mGameProtocolVersionString;
        
    }
}
