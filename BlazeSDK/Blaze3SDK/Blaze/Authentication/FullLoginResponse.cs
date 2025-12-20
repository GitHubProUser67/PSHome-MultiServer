using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct FullLoginResponse
	{

		[TdfMember("AGUP")]
		public bool mCanAgeUp;

		[TdfMember("SPAM")]
		public bool mIsOfLegalContactAge;

		[TdfMember("LDHT")]
		public string mLegalDocHost;

		[TdfMember("NTOS")]
		public bool mNeedsLegalDoc;

		[TdfMember("PCTK")]
		public string mPCLoginToken;

		[TdfMember("PRIV")]
		public string mPrivacyPolicyUri;

		[TdfMember("SESS")]
		public SessionInfo mSessionInfo;

		[TdfMember("TSUI")]
		public string mTermsOfServiceUri;

		[TdfMember("THST")]
		public string mTosHost;

		[TdfMember("TURI")]
		public string mTosUri;

	}
}
