using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerBanRequest
	{

		[TdfMember("CNTX")]
		public ushort mContext;

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
