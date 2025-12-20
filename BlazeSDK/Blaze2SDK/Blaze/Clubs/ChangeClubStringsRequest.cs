using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ChangeClubStringsRequest
    {
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("CABR")]
        [StringLength(30)]
        public string mAbbrev;
        
        /// <summary>
        /// Max String Length: 65
        /// </summary>
        [TdfMember("CDSC")]
        [StringLength(65)]
        public string mDescription;
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(30)]
        public string mName;
        
    }
}
