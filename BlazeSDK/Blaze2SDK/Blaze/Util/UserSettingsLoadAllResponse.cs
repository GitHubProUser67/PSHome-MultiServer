using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct UserSettingsLoadAllResponse
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 1024
        /// </summary>
        [TdfMember("SMAP")]
        public SortedDictionary<string, string> mDataMap;
        
    }
}
