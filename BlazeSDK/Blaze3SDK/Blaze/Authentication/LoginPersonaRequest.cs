using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct LoginPersonaRequest
	{

		[TdfMember("PNAM")]
		public string mPersonaName;

	}
}
