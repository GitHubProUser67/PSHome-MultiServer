using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetTosInfoRequest
	{

		[TdfMember("CTRY")]
		public string mIsoCountryCode;

		[TdfMember("PTFM")]
		public string mPlatform;

	}
}
