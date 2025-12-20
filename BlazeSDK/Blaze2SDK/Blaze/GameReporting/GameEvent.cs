using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameEvent
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("EATR")]
        public SortedDictionary<string, string> mAttributeMap;
        
        [TdfMember("GMET")]
        public uint mGameEventType;
        
    }
}
