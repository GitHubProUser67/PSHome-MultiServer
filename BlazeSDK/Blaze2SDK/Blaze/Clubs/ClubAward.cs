using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubAward
    {
        
        [TdfMember("AWID")]
        public uint mAwardId;
        
        /// <summary>
        /// Max String Length: 512
        /// </summary>
        [TdfMember("AWIU")]
        [StringLength(512)]
        public string mAwardImgURL;
        
        [TdfMember("CAWI")]
        public uint mCount;
        
        [TdfMember("IMCS")]
        public int mAwardImgCheckSum;
        
        [TdfMember("LUDT")]
        public uint mLastUpdateTime;
        
    }
}
