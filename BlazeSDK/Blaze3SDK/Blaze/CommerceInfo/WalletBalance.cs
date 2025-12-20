using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct WalletBalance
	{

		[TdfMember("WBAL")]
		public string mBalance;

		[TdfMember("WLCR")]
		public string mCurrency;

	}
}
