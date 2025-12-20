using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GpsContentController
{
    [TdfStruct]
    public struct FilePetitionRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ANVP")]
        public SortedDictionary<string, string> attributeMap;
        
        [TdfMember("COID")]
        public ulong mContentId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("COTY")]
        [StringLength(64)]
        public string mComplaintType;
        
        /// <summary>
        /// Max String Length: 360
        /// </summary>
        [TdfMember("PTDE")]
        [StringLength(360)]
        public string mPetitionDetail;
        
        /// <summary>
        /// Max String Length: 360
        /// </summary>
        [TdfMember("SUBJ")]
        [StringLength(360)]
        public string mSubject;
        
        /// <summary>
        /// Max String Length: 12
        /// </summary>
        [TdfMember("TMZO")]
        [StringLength(12)]
        public string mTimeZone;
        
        [TdfMember("TRGT")]
        public List<uint> mTargetUsers;
        
    }
}
