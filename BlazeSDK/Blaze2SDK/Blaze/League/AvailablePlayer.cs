using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct AvailablePlayer
    {
        
        [TdfMember("PLID")]
        public uint mPlayerId;
        
        [TdfMember("POST")]
        public uint mPosition;
        
    }
}
