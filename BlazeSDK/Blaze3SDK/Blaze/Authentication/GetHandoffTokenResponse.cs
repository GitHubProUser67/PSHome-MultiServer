using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetHandoffTokenResponse
	{

		[TdfMember("HOFF")]
		public string mHandoffToken;

	}
}
