using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct(0x3C1FCCF0)]
	public struct ReplicatedGameData
	{

		[TdfMember("ADMN")]
		public List<long> mAdminPlayerList;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteriaMap;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GNAM")]
		public string mGameName;

		[TdfMember("GPVH")]
		public ulong mGameProtocolVersionHash;

		[TdfMember("VSTR")]
		public string mGameProtocolVersionString;

		[TdfMember("GSID")]
		public ulong mGameReportingId;

		[TdfMember("GSET")]
		public GameSettings mGameSettings;

		[TdfMember("GSTA")]
		public GameState mGameState;

		[TdfMember("GTYP")]
		public string mGameTypeName;

		[TdfMember("HNET")]
		public List<NetworkAddress> mHostNetworkAddressList;

		[TdfMember("IGNO")]
		public bool mIgnoreEntryCriteriaWithInvite;

		[TdfMember("MCAP")]
		public ushort mMaxPlayerCapacity;

		[TdfMember("MATR")]
		public SortedDictionary<string, string> mMeshAttribs;

		[TdfMember("NQOS")]
		public Util.NetworkQosData mNetworkQosData;

		[TdfMember("NTOP")]
		public GameNetworkTopology mNetworkTopology;

		[TdfMember("PGID")]
		public string mPersistedGameId;

		[TdfMember("PGSR")]
		public byte[] mPersistedGameIdSecret;

		[TdfMember("PSAS")]
		public string mPingSiteAlias;

		[TdfMember("PHST")]
		public HostInfo mPlatformHostInfo;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

		[TdfMember("QCAP")]
		public ushort mQueueCapacity;

		[TdfMember("NRES")]
		public bool mServerNotResetable;

		[TdfMember("SEED")]
		public uint mSharedSeed;

		[TdfMember("CAP")]
		public List<ushort> mSlotCapacities;

		[TdfMember("TCAP")]
		public ushort mTeamCapacity;

		[TdfMember("TIDS")]
		public List<ushort> mTeamIds;

		[TdfMember("THST")]
		public HostInfo mTopologyHostInfo;

		[TdfMember("HSES")]
		public uint mTopologyHostSessionId;

		[TdfMember("UUID")]
		public string mUUID;

		[TdfMember("VOIP")]
		public VoipTopology mVoipNetwork;

		[TdfMember("XNNC")]
		public byte[] mXnetNonce;

		[TdfMember("XSES")]
		public byte[] mXnetSession;

	}
}
