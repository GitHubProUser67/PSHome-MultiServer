using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct ReportTournamentResultsRequest
    {
        
        [TdfMember("TID")]
        public uint mTournId;
        
        [TdfMember("UOID")]
        public uint mUserOneId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("UOME")]
        [StringLength(256)]
        public string mUserOneMetaData;
        
        [TdfMember("UOSC")]
        public uint mUserOneScore;
        
        [TdfMember("UOTE")]
        public int mUserOneTeam;
        
        [TdfMember("UTID")]
        public uint mUserTwoId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("UTME")]
        [StringLength(256)]
        public string mUserTwoMetaData;
        
        [TdfMember("UTSC")]
        public uint mUserTwoScore;
        
        [TdfMember("UTTE")]
        public int mUserTwoTeam;
        
    }
}
