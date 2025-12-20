using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct Category
	{

		[TdfMember("LAMP")]
		public SortedDictionary<string, string> mAttribs;

		[TdfMember("DLOC")]
		public uint mDefaultLocale;

		[TdfMember("CGID")]
		public string mId;

		[TdfMember("TCTE")]
		public bool mIsTopCategory;

		[TdfMember("PCNT")]
		public uint mProductCount;

		[TdfMember("SLST")]
		public List<string> mSubCategoryList;

		[TdfMember("CTYP")]
		public string mType;

	}
}
