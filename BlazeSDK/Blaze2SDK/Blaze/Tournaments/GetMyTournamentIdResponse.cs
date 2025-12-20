using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetMyTournamentIdResponse
    {
        
        [TdfMember("ACTI")]
        public bool isActive;
        
        [TdfMember("TNID")]
        public uint mId;
        
    }
}
