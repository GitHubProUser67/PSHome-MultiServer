using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyPlayerAttribChange
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mPlayerAttribs;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
    }
}
