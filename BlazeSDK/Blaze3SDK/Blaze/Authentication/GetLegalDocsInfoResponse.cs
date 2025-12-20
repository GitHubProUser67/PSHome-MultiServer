using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetLegalDocsInfoResponse
	{

		[TdfMember("EAMC")]
		public uint mEaMayContact;

		[TdfMember("LHST")]
		public string mLegalDocHost;

		[TdfMember("PMC")]
		public uint mPartnersMayContact;

		[TdfMember("PPUI")]
		public string mPrivacyPolicyUri;

		[TdfMember("TSUI")]
		public string mTermsOfServiceUri;

	}
}
