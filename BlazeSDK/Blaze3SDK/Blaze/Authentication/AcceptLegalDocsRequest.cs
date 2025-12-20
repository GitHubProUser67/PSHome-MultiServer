using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct AcceptLegalDocsRequest
	{

		[TdfMember("PRIV")]
		public string mPrivacyPolicyUri;

		[TdfMember("TSUI")]
		public string mTermsOfServiceUri;

	}
}
