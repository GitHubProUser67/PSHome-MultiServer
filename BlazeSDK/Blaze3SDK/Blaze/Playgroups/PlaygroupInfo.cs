using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct(0xAADA08AF)]
	public struct PlaygroupInfo
	{

		[TdfMember("ENBV")]
		public bool mEnableVoIP;

		[TdfMember("UPRS")]
		public bool mHasPresence;

		[TdfMember("HNET")]
		public NetworkAddress mHostNetworkAddress;

		[TdfMember("HSID")]
		public byte mHostSlotId;

		[TdfMember("PGID")]
		public uint mId;

		[TdfMember("MLIM")]
		public ushort mMaxMembers;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("NTOP")]
		public GameNetworkTopology mNetworkTopology;

		[TdfMember("OWNR")]
		public long mOwnerBlazeId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlaygroupAttributes;

		[TdfMember("JOIN")]
		public PlaygroupJoinability mPlaygroupJoinability;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

		[TdfMember("UUID")]
		public string mUUID;

		[TdfMember("UKEY")]
		public string mUniqueKey;

		[TdfMember("VOIP")]
		public VoipTopology mVoipNetwork;

		[TdfMember("XNNC")]
		public byte[] mXnetNonce;

		[TdfMember("XSES")]
		public byte[] mXnetSession;

	}
}
