using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetMyTournamentDetailsResponse
    {
        
        [TdfMember("ACTI")]
        public bool isActive;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTR")]
        [StringLength(32)]
        public string mTournAttribute;
        
        [TdfMember("LEVL")]
        public uint mLevel;
        
        [TdfMember("TEAM")]
        public int mTeam;
        
        [TdfMember("TID")]
        public uint mTournamentId;
        
        [TdfMember("TREE")]
        public List<TournamentNode> mTournamentTree;
        
    }
}
