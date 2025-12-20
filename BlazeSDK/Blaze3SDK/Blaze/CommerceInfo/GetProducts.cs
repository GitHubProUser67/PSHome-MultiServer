using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetProducts
	{

		[TdfMember("CLNM")]
		public string mCatalogName;

		[TdfMember("CGNM")]
		public string mCategoryName;

		[TdfMember("PPSN")]
		public ushort mPageNo;

		[TdfMember("PPSZ")]
		public ushort mPageSize;

	}
}
