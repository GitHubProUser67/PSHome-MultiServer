using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct UpdateRegistrationGameIncrementRequest
    {
        
        [TdfMember("EVID")]
        public int mEvid;
        
        [TdfMember("NUMG")]
        public int mNumGames;
        
        [TdfMember("UID")]
        public uint mUserId;
        
    }
}
