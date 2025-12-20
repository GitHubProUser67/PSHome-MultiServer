using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetDateRangeRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        [TdfMember("POFF")]
        public int mPeriodOffset;
        
        [TdfMember("PTYP")]
        public int mPeriodType;
        
    }
}
