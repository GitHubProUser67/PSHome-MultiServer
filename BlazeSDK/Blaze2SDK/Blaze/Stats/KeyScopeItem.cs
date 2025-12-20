using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct KeyScopeItem
    {
        
        [TdfMember("KSIT")]
        public KeyScopeType mKeyScopeType;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("KSVL")]
        public List<string> mKeyScopeList;
        
    }
}
