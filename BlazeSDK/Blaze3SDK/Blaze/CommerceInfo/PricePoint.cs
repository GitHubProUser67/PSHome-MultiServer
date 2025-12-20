using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct PricePoint
	{

		[TdfMember("PCUY")]
		public string mCurrency;

		[TdfMember("PCTP")]
		public string mCurrencyType;

		[TdfMember("PPLC")]
		public string mLocale;

		[TdfMember("PP")]
		public string mPrice;

		[TdfMember("PPT")]
		public string mPriceType;

	}
}
