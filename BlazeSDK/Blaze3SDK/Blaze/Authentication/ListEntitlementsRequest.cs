using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ListEntitlementsRequest
	{

		[TdfMember("FLAG")]
		public EntitlementSearchFlag mEntitlementSearchFlag;

		[TdfMember("GNLS")]
		public List<string> mGroupNameList;

		[TdfMember("EPSN")]
		public ushort mPageNo;

		[TdfMember("EPSZ")]
		public ushort mPageSize;

		[TdfMember("ONLY")]
		public bool mRestrictive;

		[TdfMember("BUID")]
		public long mUserId;

	}
}
