using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct UserProfileInfo
	{

		[TdfMember("CITY")]
		public string mCity;

		[TdfMember("CTRY")]
		public string mCountry;

		[TdfMember("GNDR")]
		public Gender mGender;

		[TdfMember("STAT")]
		public string mState;

		[TdfMember("STRT")]
		public string mStreet;

		[TdfMember("ZIP")]
		public string mZipCode;

	}
}
