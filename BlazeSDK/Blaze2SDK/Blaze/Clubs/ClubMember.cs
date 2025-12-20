using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubMember
    {
        
        [TdfMember("BLID")]
        public uint mBlazeId;
        
        [TdfMember("CMTP")]
        public MembershipStatus mMembershipStatus;
        
        [TdfMember("MBOS")]
        public MemberOnlineStatus mOnlineStatus;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("META")]
        public SortedDictionary<string, string> mMetaData;
        
        [TdfMember("MSTM")]
        public uint mMembershipSinceTime;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(256)]
        public string mPersona;
        
    }
}
