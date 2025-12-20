using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubMembership
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("MBER")]
        public ClubMember mClubMember;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(30)]
        public string mClubName;
        
    }
}
