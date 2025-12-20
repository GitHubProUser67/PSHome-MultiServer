using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct KeyScopes
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("KSIT")]
        public SortedDictionary<string, KeyScopeItem> mKeyScopesMap;
        
    }
}
