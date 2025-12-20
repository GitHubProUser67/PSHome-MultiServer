using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetWalletBalance
	{

		[TdfMember("WLNM")]
		public string mWalletName;

	}
}
