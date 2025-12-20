using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct GetStatDescsRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("STAT")]
        public List<string> mStatNames;
        
    }
}
