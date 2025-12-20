using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct MatchmakingCriteriaError
    {
        
        /// <summary>
        /// Max String Length: 160
        /// </summary>
        [TdfMember("MSG")]
        [StringLength(160)]
        public string mErrMessage;
        
    }
}
