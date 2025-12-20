using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubLocalizedNews
    {
        
        [TdfMember("CLID")]
        public uint mAssociateClubId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NTXT")]
        [StringLength(256)]
        public string mText;
        
        [TdfMember("NWCC")]
        public uint mContentCreator;
        
        [TdfMember("NWFL")]
        public ClubNewsFlags mFlags;
        
        [TdfMember("NWID")]
        public ulong mNewsId;
        
        [TdfMember("NWTY")]
        public NewsType mType;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(256)]
        public string mPersona;
        
        [TdfMember("TMST")]
        public uint mTimestamp;
        
    }
}
