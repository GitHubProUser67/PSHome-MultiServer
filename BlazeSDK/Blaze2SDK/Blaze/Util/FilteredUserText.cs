using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct FilteredUserText
    {
        
        [TdfMember("DIRT")]
        public FilterResult mResult;
        
        /// <summary>
        /// Max String Length: 512
        /// </summary>
        [TdfMember("UTXT")]
        [StringLength(512)]
        public string mFilteredText;
        
    }
}
