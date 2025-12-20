using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubInfo
    {
        
        [TdfMember("AWCN")]
        public uint mAwardCount;
        
        [TdfMember("CIMC")]
        public uint mMemberCount;
        
        [TdfMember("CRTI")]
        public uint mCreationTime;
        
        [TdfMember("GMCN")]
        public uint mGmCount;
        
        [TdfMember("LATI")]
        public uint mLastActiveTime;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("LSGR")]
        [StringLength(32)]
        public string mLastGameResult;
        
        [TdfMember("LSOP")]
        public uint mLastOppo;
        
        [TdfMember("MSCO")]
        public SortedDictionary<MemberOnlineStatus, ushort> mMemberOnlineStatusCounts;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("OPNM")]
        [StringLength(30)]
        public string mLastOppoName;
        
        [TdfMember("RVCN")]
        public uint mRivalCount;
        
    }
}
