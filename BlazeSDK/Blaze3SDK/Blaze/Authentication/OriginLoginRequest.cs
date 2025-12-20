using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct OriginLoginRequest
	{

		[TdfMember("AUTH")]
		public string mAuthToken;

		[TdfMember("TYPE")]
		public TOKENTYPE mTokenType;

	}
}
