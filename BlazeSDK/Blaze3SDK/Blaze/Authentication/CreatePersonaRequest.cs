using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CreatePersonaRequest
	{

		[TdfMember("PNAM")]
		public string mPersonaName;

	}
}
