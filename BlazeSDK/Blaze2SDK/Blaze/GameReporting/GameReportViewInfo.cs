using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportViewInfo
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(128)]
        public string mDesc;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("META")]
        [StringLength(128)]
        public string mMetadata;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("VNAM")]
        [StringLength(64)]
        public string mName;
        
    }
}
