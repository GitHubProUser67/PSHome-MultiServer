using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct LoginResponse
	{

		[TdfMember("SPAM")]
		public bool mIsOfLegalContactAge;

		[TdfMember("LDHT")]
		public string mLegalDocHost;

		[TdfMember("NTOS")]
		public bool mNeedsLegalDoc;

		[TdfMember("PCTK")]
		public string mPCLoginToken;

		[TdfMember("PLST")]
		public List<PersonaDetails> mPersonaDetailsList;

		[TdfMember("PRIV")]
		public string mPrivacyPolicyUri;

		[TdfMember("SKEY")]
		public string mSessionKey;

		[TdfMember("TSUI")]
		public string mTermsOfServiceUri;

		[TdfMember("THST")]
		public string mTosHost;

		[TdfMember("TURI")]
		public string mTosUri;

		[TdfMember("UID")]
		public long mUserId;

	}
}
