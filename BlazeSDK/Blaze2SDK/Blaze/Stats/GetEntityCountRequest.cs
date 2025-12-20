using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetEntityCountRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("KSUM")]
        public SortedDictionary<string, string> mKeyScopeValueMap;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        [TdfMember("PTYP")]
        public int mPeriodType;
        
    }
}
