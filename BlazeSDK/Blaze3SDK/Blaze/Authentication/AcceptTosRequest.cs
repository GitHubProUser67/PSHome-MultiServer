using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct AcceptTosRequest
	{

		[TdfMember("TURI")]
		public string mTosUri;

	}
}
