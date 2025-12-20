using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubRecord
    {
        
        [TdfMember("BLID")]
        public uint mBlazeId;
        
        [TdfMember("LUDT")]
        public uint mLastUpdateTime;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(256)]
        public string mPersona;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("RCDC")]
        [StringLength(128)]
        public string mRecordDescription;
        
        [TdfMember("RCID")]
        public uint mRecordId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("RCNM")]
        [StringLength(32)]
        public string mRecordName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("STAT")]
        [StringLength(32)]
        public string mRecordStat;
        
        [TdfMember("STYP")]
        public RecordStatType mRecordStatType;
        
    }
}
