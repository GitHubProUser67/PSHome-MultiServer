using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct Catalog
	{

		[TdfMember("LAMP")]
		public SortedDictionary<string, string> mAttribs;

		[TdfMember("CCUY")]
		public string mCurrency;

		[TdfMember("CCTP")]
		public string mCurrencyType;

		[TdfMember("CGID")]
		public string mId;

	}
}
