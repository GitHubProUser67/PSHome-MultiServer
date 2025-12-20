using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct ListPurchasesRequest
	{

		[TdfMember("HIST")]
		public bool mIncludeHistory;

		[TdfMember("LCAP")]
		public ushort mListCapacity;

		[TdfMember("UID")]
		public long mUserId;

	}
}
