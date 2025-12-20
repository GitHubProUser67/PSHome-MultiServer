using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct PreAuthResponse
	{

		[TdfMember("ANON")]
		public bool mAnonymousChildAccountsEnabled;

		[TdfMember("ASRC")]
		public string mAuthenticationSource;

		[TdfMember("CIDS")]
		public List<ushort> mComponentIds;

		[TdfMember("CONF")]
		public FetchConfigResponse mConfig;

		[TdfMember("INST")]
		public string mInstanceName;

		[TdfMember("PILD")]
		public string mLegalDocGameIdentifier;

		[TdfMember("CNGN")]
		public string mParentalConsentEntitlementGroupName;

		[TdfMember("PTAG")]
		public string mParentalConsentEntitlementTag;

		[TdfMember("NASP")]
		public string mPersonaNamespace;

		[TdfMember("PLAT")]
		public string mPlatform;

		[TdfMember("QOSS")]
		public QosConfigInfo mQosSettings;

		[TdfMember("RSRC")]
		public string mRegistrationSource;

		[TdfMember("SVER")]
		public string mServerVersion;

		[TdfMember("MINR")]
		public bool mUnderageSupported;

	}
}
