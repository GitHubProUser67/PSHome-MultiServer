using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GetFullGameDataRequest
    {
        
        [TdfMember("GIDL")]
        public List<uint> mGameIdList;
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("PIDL")]
        public List<string> mPersistedGameIdList;
        
    }
}
