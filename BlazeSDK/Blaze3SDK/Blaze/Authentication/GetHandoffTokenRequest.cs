using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetHandoffTokenRequest
	{

		[TdfMember("CSTR")]
		public string mClientString;

	}
}
