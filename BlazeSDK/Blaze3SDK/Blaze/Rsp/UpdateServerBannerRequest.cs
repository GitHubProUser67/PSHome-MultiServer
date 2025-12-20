using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerBannerRequest
	{

		[TdfMember("BID")]
		public int mBannerId;

		[TdfMember("CLR")]
		public bool mClearBannerId;

		[TdfMember("SID")]
		public uint mServerId;

	}
}
