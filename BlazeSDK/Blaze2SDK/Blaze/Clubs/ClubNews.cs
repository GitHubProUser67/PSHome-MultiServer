using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubNews
    {
        
        [TdfMember("CLID")]
        public uint mAssociateClubId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NPRL")]
        [StringLength(256)]
        public string mParamList;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NSIS")]
        [StringLength(64)]
        public string mStringId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NTXT")]
        [StringLength(256)]
        public string mText;
        
        [TdfMember("NWCC")]
        public uint mContentCreator;
        
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
