using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GetGameReportViewInfo
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VNAM")]
        [StringLength(64)]
        public string mName;
        
    }
}
