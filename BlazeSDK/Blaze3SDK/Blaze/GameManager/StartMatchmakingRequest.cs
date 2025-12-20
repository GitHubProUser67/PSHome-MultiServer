using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct StartMatchmakingRequest
	{

		[TdfMember("CRIT")]
		public MatchmakingCriteriaData mCriteriaData;

		[TdfMember("ECRI")]
		public SortedDictionary<string, string> mEntryCriteriaMap;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("GENT")]
		public GameEntryType mGameEntryType;

		[TdfMember("GNAM")]
		public string mGameName;

		[TdfMember("GVER")]
		public string mGameProtocolVersionString;

		[TdfMember("GSET")]
		public GameSettings mGameSettings;

		[TdfMember("BTPL")]
		public BlazeObjectId mGroupId;

		[TdfMember("IGNO")]
		public bool mIgnoreEntryCriteriaWithInvite;

		[TdfMember("PMAX")]
		public ushort mMaxPlayerCapacity;

		[TdfMember("NTOP")]
		public GameNetworkTopology mNetworkTopology;

		[TdfMember("PATT")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PLST")]
		public List<long> mPlayerIdList;

		[TdfMember("PNET")]
		public NetworkAddress mPlayerNetworkAddress;

		[TdfMember("QCAP")]
		public ushort mQueueCapacity;

		[TdfMember("DUR")]
		public uint mSessionDurationMS;

		[TdfMember("SIDL")]
		public List<uint> mSessionIdList;

		[TdfMember("MODE")]
		public MatchmakingSessionMode mSessionMode;

		[TdfMember("VOIP")]
		public VoipTopology mVoipNetwork;

	}
}
