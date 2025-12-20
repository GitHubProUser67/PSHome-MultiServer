using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetAuthTokenResponse
	{

		[TdfMember("AUTH")]
		public string mAuthToken;

	}
}
