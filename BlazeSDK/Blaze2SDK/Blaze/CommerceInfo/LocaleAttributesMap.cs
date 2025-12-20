using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct LocaleAttributesMap
    {
        
        /// <summary>
        /// Max Key String Length: 255
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("LAMP")]
        public SortedDictionary<string, string> mLocaleAttributeMap;
        
    }
}
