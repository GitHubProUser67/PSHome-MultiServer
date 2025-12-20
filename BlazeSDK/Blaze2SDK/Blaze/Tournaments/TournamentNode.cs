using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct TournamentNode
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTO")]
        [StringLength(32)]
        public string mUserOneAttribute;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTT")]
        [StringLength(32)]
        public string mUserTwoAttribute;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("METO")]
        [StringLength(64)]
        public string mUserOneMetaData;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("METT")]
        [StringLength(64)]
        public string mUserTwoMetaData;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAMO")]
        [StringLength(256)]
        public string mUserOneName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAMT")]
        [StringLength(256)]
        public string mUserTwoName;
        
        [TdfMember("NOID")]
        public uint mNodeId;
        
        [TdfMember("SCOO")]
        public uint mUserOneScore;
        
        [TdfMember("SCOT")]
        public uint mUserTwoScore;
        
        [TdfMember("TEAO")]
        public int mUserOneTeam;
        
        [TdfMember("TEAT")]
        public int mUserTwoTeam;
        
        [TdfMember("UIDO")]
        public uint mUserOneId;
        
        [TdfMember("UIDT")]
        public uint mUserTwoId;
        
    }
}
