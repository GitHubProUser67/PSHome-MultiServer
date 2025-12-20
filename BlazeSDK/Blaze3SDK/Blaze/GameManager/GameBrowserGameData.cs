using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameBrowserGameData
	{

		[TdfMember("ADMN")]
		public List<long> mAdminPlayerList;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteriaMap;

		[TdfMember("SID")]
		public ulong mExternalSessionId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("TINF")]
		public List<GameBrowserTeamInfo> mGameBrowserTeamInfoVector;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GNAM")]
		public string mGameName;

		[TdfMember("VSTR")]
		public string mGameProtocolVersionString;

		[TdfMember("ROST")]
		public List<GameBrowserPlayerData> mGameRoster;

		[TdfMember("GSET")]
		public GameSettings mGameSettings;

		[TdfMember("GSTA")]
		public GameState mGameState;

		[TdfMember("HOST")]
		public long mHostId;

		[TdfMember("HNET")]
		public List<NetworkAddress> mHostNetworkAddressList;

		[TdfMember("NTOP")]
		public GameNetworkTopology mNetworkTopology;

		[TdfMember("PSID")]
		public string mPersistedGameId;

		[TdfMember("PSAS")]
		public string mPingSiteAlias;

		[TdfMember("PCNT")]
		public List<ushort> mPlayerCounts;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

		[TdfMember("QCAP")]
		public ushort mQueueCapacity;

		[TdfMember("QCNT")]
		public ushort mQueueCount;

		[TdfMember("CAP")]
		public List<ushort> mSlotCapacities;

		[TdfMember("TCAP")]
		public ushort mTeamCapacity;

		[TdfMember("VOIP")]
		public VoipTopology mVoipTopology;

	}
}
