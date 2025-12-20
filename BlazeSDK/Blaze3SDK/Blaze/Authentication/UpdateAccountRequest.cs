using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct UpdateAccountRequest
	{

		[TdfMember("CTRY")]
		public string mCountry;

		[TdfMember("CPWD")]
		public string mCurrentPassword;

		[TdfMember("DOB")]
		public string mDOB;

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("OPT1")]
		public byte mGlobalOptin;

		[TdfMember("LANG")]
		public string mLanguage;

		[TdfMember("PRNT")]
		public string mParentalEmail;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("OPT3")]
		public byte mThirdPartyOptin;

		[TdfMember("UID")]
		public long mUserId;

	}
}
