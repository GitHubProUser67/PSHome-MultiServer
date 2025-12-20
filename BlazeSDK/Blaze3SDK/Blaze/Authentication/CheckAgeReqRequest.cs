using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CheckAgeReqRequest
	{

		[TdfMember("BDAY")]
		public int mBirthDay;

		[TdfMember("BMON")]
		public int mBirthMonth;

		[TdfMember("BYR")]
		public int mBirthYear;

		[TdfMember("CTRY")]
		public string mIsoCountryCode;

	}
}
