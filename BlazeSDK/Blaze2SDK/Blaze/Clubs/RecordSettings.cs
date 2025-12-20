using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct RecordSettings
    {
        
        [TdfMember("RCID")]
        public uint mRecordId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("RCNA")]
        [StringLength(64)]
        public string mRecordName;
        
    }
}
