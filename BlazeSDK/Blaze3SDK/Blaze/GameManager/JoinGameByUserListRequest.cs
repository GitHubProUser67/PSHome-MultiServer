using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct JoinGameByUserListRequest
	{

		[TdfMember("GENT")]
		public GameEntryType mGameEntryType;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GVER")]
		public string mGameProtocolVersionString;

		[TdfMember("JMET")]
		public JoinMethod mJoinMethod;

		[TdfMember("TIDX")]
		public ushort mJoiningTeamIndex;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PLST")]
		public List<long> mPlayerIdList;

		[TdfMember("PNET")]
		public NetworkAddress mPlayerNetworkAddress;

		[TdfMember("SLOT")]
		public SlotType mRequestedSlotType;

	}
}
