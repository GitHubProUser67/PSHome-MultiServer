using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct PersonaRequest
    {
        
        [TdfMember("PID")]
        public long mPersonaId;
        
    }
}
