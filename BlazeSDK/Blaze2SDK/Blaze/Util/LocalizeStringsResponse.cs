using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct LocalizeStringsResponse
    {
        
        /// <summary>
        /// Max Key String Length: 64
        /// Max Value String Length: 512
        /// </summary>
        [TdfMember("SMAP")]
        public SortedDictionary<string, string> mLocalizedStrings;
        
    }
}
