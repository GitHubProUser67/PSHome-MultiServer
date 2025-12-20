using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct UpdateGameSessionRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("XNNC")]
        public byte[] mXnetNonce;
        
        [TdfMember("XSES")]
        public byte[] mXnetSession;
        
    }
}
