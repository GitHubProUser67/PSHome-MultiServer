using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetTosInfoResponse
	{

		[TdfMember("EAMC")]
		public uint mEaMayContact;

		[TdfMember("PMC")]
		public uint mPartnersMayContact;

		[TdfMember("PRIV")]
		public string mPrivacyPolicyUri;

		[TdfMember("THST")]
		public string mTosHost;

		[TdfMember("TURI")]
		public string mTosUri;

	}
}
