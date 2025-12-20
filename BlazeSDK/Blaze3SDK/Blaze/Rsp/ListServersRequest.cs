using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct ListServersRequest
	{

		[TdfMember("IADM")]
		public bool mIncludeAdminServers;

		[TdfMember("HIST")]
		public bool mIncludeHistory;

		[TdfMember("UID")]
		public long mUserId;

	}
}
