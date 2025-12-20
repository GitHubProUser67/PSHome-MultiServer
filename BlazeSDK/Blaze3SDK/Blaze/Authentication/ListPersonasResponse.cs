using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ListPersonasResponse
	{

		[TdfMember("PINF")]
		public List<PersonaDetails> mList;

	}
}
