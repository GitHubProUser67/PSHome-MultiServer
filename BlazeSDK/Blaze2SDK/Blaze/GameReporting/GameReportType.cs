using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportType
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ATYP")]
        public List<string> mAttributeTypes;
        
        [TdfMember("GTID")]
        public uint mGameTypeId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GTNA")]
        [StringLength(64)]
        public string mGameTypeName;
        
    }
}
