using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct UserDetails
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("PLST")]
		public List<PersonaDetails> mPersonaDetailsList;

	}
}
