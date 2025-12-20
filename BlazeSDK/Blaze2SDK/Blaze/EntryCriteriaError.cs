using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct EntryCriteriaError
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("FCRT")]
        [StringLength(32)]
        public string mFailedCriteria;
        
    }
}
