using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ListPersonasResponse
    {
        
        [TdfMember("PINF")]
        public List<PersonaDetails> mList;
        
    }
}
