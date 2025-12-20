using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct CheckEntryCriteriaResponse
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("FCRI")]
        [StringLength(64)]
        public string mFailedCriteria;
        
        [TdfMember("PASS")]
        public bool mPassed;
        
    }
}
