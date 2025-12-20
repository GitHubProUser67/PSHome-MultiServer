using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ExpressLoginRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("PNAM")]
		public string mPersonaName;

	}
}
