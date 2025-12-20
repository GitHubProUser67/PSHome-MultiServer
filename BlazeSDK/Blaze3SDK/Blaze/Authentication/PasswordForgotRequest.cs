using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PasswordForgotRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

	}
}
