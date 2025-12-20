using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerAdminRequest
	{

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
