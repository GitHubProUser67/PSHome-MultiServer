using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetPersonaResponse
	{

		[TdfMember("PINF")]
		public PersonaInfo mPersonaInfo;

		[TdfMember("UID")]
		public long mUserId;

	}
}
