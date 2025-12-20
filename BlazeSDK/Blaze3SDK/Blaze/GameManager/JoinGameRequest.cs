using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct JoinGameRequest
	{

		[TdfMember("PLST")]
		public List<long> mAdditionalPlayerIdList;

		[TdfMember("GENT")]
		public GameEntryType mGameEntryType;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GVER")]
		public string mGameProtocolVersionString;

		[TdfMember("BTPL")]
		public BlazeObjectId mGroupId;

		[TdfMember("JMET")]
		public JoinMethod mJoinMethod;

		[TdfMember("TIDX")]
		public ushort mJoiningTeamIndex;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PNET")]
		public NetworkAddress mPlayerNetworkAddress;

		[TdfMember("SLID")]
		public byte mRequestedSlotId;

		[TdfMember("SLOT")]
		public SlotType mRequestedSlotType;

		[TdfMember("USER")]
		public UserIdentification mUser;

	}
}
