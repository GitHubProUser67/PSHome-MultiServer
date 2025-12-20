using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetLegalDocsInfoRequest
	{

		[TdfMember("CTRY")]
		public string mIsoCountryCode;

		[TdfMember("PTFM")]
		public string mPlatform;

	}
}
