using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct CreateClubRequest
    {
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(30)]
        public string mName;
        
        [TdfMember("CSET")]
        public ClubSettings mClubSettings;
        
    }
}
