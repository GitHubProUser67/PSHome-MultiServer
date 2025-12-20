using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct AwardSettings
    {
        
        [TdfMember("AWCS")]
        public uint mAwardChecksum;
        
        [TdfMember("AWID")]
        public uint mAwardId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("AWNA")]
        [StringLength(64)]
        public string mAwardName;
        
        /// <summary>
        /// Max String Length: 512
        /// </summary>
        [TdfMember("AWUR")]
        [StringLength(512)]
        public string mAwardURL;
        
    }
}
