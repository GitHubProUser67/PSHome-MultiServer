using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ValidateSessionKeyRequest
	{

		[TdfMember("SKEY")]
		public string mSessionKey;

	}
}
