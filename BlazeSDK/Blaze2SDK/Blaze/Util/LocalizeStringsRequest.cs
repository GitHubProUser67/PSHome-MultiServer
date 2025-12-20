using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct LocalizeStringsRequest
    {
        
        [TdfMember("LANG")]
        public uint mLocale;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("LSID")]
        public List<string> mStringIds;
        
    }
}
