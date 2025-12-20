using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameCreated
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
    }
}
