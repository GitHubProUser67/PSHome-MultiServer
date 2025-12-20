using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CreateAccountParameters
	{

		[TdfMember("BDAY")]
		public int mBirthDay;

		[TdfMember("BMON")]
		public int mBirthMonth;

		[TdfMember("BYR")]
		public int mBirthYear;

		[TdfMember("DVID")]
		public ulong mDeviceId;

		[TdfMember("OPT1")]
		public byte mEaEmailAllowed;

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("GEST")]
		public bool mIsGuest;

		[TdfMember("CTRY")]
		public string mIsoCountryCode;

		[TdfMember("LANG")]
		public string mIsoLanguageCode;

		[TdfMember("PRNT")]
		public string mParentalEmail;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("PNAM")]
		public string mPersonaName;

		[TdfMember("PRIV")]
		public string mPrivacyPolicyUri;

		[TdfMember("TSUI")]
		public string mTermsOfServiceUri;

		[TdfMember("OPT3")]
		public byte mThirdPartyEmailAllowed;

		[TdfMember("TOSV")]
		public string mTosVersion;

		[TdfMember("PROF")]
		public UserProfileInfo mUserProfileInfo;

	}
}
