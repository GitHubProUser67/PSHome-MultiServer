using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct AccountInfo
	{

		[TdfMember("AMU")]
		public bool mAnonymousUser;

		[TdfMember("ASRC")]
		public string mAuthenticationSource;

		[TdfMember("CO")]
		public string mCountry;

		[TdfMember("DOB")]
		public string mDOB;

		[TdfMember("DTCR")]
		public string mDateCreated;

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("STAT")]
		public EmailStatus mEmailStatus;

		[TdfMember("GOPT")]
		public byte mGlobalOptin;

		[TdfMember("LN")]
		public string mLanguage;

		[TdfMember("LATH")]
		public string mLastAuth;

		[TdfMember("PML")]
		public string mParentalEmail;

		[TdfMember("RC")]
		public StatusReason mReasonCode;

		[TdfMember("STAS")]
		public AccountStatus mStatus;

		[TdfMember("TPOT")]
		public byte mThirdPartyOptin;

		[TdfMember("TOSV")]
		public string mTosVersion;

		[TdfMember("UDU")]
		public bool mUnderageUser;

		[TdfMember("UID")]
		public long mUserId;

	}
}
