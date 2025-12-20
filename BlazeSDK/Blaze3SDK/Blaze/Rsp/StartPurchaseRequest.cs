using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct StartPurchaseRequest
	{

		[TdfMember("CID")]
		public uint mConsumableId;

		[TdfMember("PSAL")]
		public string mPingSiteAlias;

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("UID")]
		public long mUserId;

	}
}
