using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct JoinPlaygroupRequest
	{

		[TdfMember("PGID")]
		public uint mId;

		[TdfMember("PNET")]
		public NetworkAddress mNetworkAddress;

		[TdfMember("UKEY")]
		public string mUniqueKey;

		[TdfMember("USER")]
		public UserIdentification mUser;

	}
}
