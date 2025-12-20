using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct(0xC86D3EA7)]
	public struct ReplicatedGamePlayer
	{

		[TdfMember("LOC")]
		public uint mAccountLocale;

		[TdfMember("BLOB")]
		public byte[] mCustomData;

		[TdfMember("EXID")]
		public ulong mExternalId;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("TIME")]
		public long mJoinedGameTimestamp;

		[TdfMember("PNET")]
		public NetworkAddress mNetworkAddress;

		[TdfMember("PATT")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("NAME")]
		public string mPlayerName;

		[TdfMember("UID")]
		public uint mPlayerSessionId;

		[TdfMember("STAT")]
		public PlayerState mPlayerState;

		[TdfMember("SID")]
		public byte mSlotId;

		[TdfMember("SLOT")]
		public SlotType mSlotType;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

		[TdfMember("UGID")]
		public BlazeObjectId mUserGroupId;

	}
}
