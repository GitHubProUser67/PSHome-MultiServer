using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateGameBanRequest
	{

		[TdfMember("CNTX")]
		public ushort mContext;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
