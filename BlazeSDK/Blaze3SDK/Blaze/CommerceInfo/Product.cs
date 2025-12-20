using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct Product
	{

		[TdfMember("PATT")]
		public SortedDictionary<string, string> mAttribs;

		[TdfMember("DLOC")]
		public uint mDefaultLocale;

		[TdfMember("FID")]
		public string mFinanceId;

		[TdfMember("PID")]
		public string mId;

		[TdfMember("PNAM")]
		public string mName;

		[TdfMember("PLFM")]
		public string mPlatForm;

		[TdfMember("PPP")]
		public PricePoints mPricePoints;

		[TdfMember("PTPE")]
		public string mType;

	}
}
