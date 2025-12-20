using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct(0x6F8E65A9)]
	public struct PlaygroupMemberInfo
	{

		[TdfMember("JTIM")]
		public uint mJoinTime;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mMemberAttributes;

		[TdfMember("PNET")]
		public NetworkAddress mNetworkAddress;

		[TdfMember("PERM")]
		public MemberPermissions mPermissions;

		[TdfMember("SID")]
		public byte mSlotId;

		[TdfMember("USER")]
		public UserIdentification mUser;

	}
}
