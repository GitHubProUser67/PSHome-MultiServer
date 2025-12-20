using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerVipRequest
	{

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
