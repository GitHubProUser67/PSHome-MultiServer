using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct SilentLoginRequest
	{

		[TdfMember("AUTH")]
		public string mAuthToken;

		[TdfMember("PID")]
		public long mPersonaId;

		[TdfMember("TYPE")]
		public TOKENTYPE mTokenType;

	}
}
