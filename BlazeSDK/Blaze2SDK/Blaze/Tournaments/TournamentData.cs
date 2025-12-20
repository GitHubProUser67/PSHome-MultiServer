using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct TournamentData
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(128)]
        public string mDescription;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mName;
        
        [TdfMember("NMEM")]
        public uint mTotalMemberCount;
        
        [TdfMember("NOMM")]
        public uint mOnlineMemberCount;
        
        [TdfMember("RNDS")]
        public uint mNumRounds;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("TMET")]
        [StringLength(64)]
        public string mTrophyMetaData;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TNAM")]
        [StringLength(32)]
        public string mTrophyName;
        
        [TdfMember("TNID")]
        public uint mId;
        
    }
}
