using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CheckAgeReqResponse
	{

		[TdfMember("SPAM")]
		public byte mIsOfLegalContactAge;

		[TdfMember("ANON")]
		public bool mMustBeAnonymous;

		[TdfMember("PEND")]
		public byte mPendingParentalConsent;

	}
}
