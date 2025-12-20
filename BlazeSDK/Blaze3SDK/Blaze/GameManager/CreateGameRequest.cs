using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct CreateGameRequest
	{

		[TdfMember("ADMN")]
		public List<long> mAdminPlayerList;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteriaMap;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("GENT")]
		public GameEntryType mGameEntryType;

		[TdfMember("GNAM")]
		public string mGameName;

		[TdfMember("GCTR")]
		public string mGamePingSiteAlias;

		[TdfMember("VSTR")]
		public string mGameProtocolVersionString;

		[TdfMember("GSET")]
		public GameSettings mGameSettings;

		[TdfMember("GURL")]
		public string mGameStatusUrl;

		[TdfMember("GTYP")]
		public string mGameTypeName;

		[TdfMember("BTPL")]
		public BlazeObjectId mGroupId;

		[TdfMember("HNET")]
		public List<NetworkAddress> mHostNetworkAddressList;

		[TdfMember("PATT")]
		public SortedDictionary<string, string> mHostPlayerAttribs;

		[TdfMember("IGNO")]
		public bool mIgnoreEntryCriteriaWithInvite;

		[TdfMember("SLOT")]
		public SlotType mJoiningSlotType;

		[TdfMember("TIDX")]
		public ushort mJoiningTeamIndex;

		[TdfMember("PMAX")]
		public ushort mMaxPlayerCapacity;

		[TdfMember("MATR")]
		public SortedDictionary<string, string> mMeshAttribs;

		[TdfMember("NTOP")]
		public GameNetworkTopology mNetworkTopology;

		[TdfMember("PGID")]
		public string mPersistedGameId;

		[TdfMember("PGSC")]
		public byte[] mPersistedGameIdSecret;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

		[TdfMember("QCAP")]
		public ushort mQueueCapacity;

		[TdfMember("RGID")]
		public uint mReservedDynamicDSGameId;

		[TdfMember("SEAT")]
		public List<long> mReservedPlayerSeats;

		[TdfMember("NRES")]
		public bool mServerNotResetable;

		[TdfMember("SIDL")]
		public List<uint> mSessionIdList;

		[TdfMember("PCAP")]
		public List<ushort> mSlotCapacities;

		[TdfMember("TCAP")]
		public ushort mTeamCapacity;

		[TdfMember("TIDS")]
		public List<ushort> mTeamIds;

		[TdfMember("VOIP")]
		public VoipTopology mVoipNetwork;

	}
}
