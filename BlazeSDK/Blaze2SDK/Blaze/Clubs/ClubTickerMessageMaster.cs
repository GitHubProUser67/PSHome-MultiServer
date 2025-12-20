using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubTickerMessageMaster
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CTMS")]
        public ClubTickerMessage mMessage;
        
        [TdfMember("EXUI")]
        public uint mExcludeUserId;
        
        [TdfMember("INUI")]
        public uint mIncludeUserId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PRMS")]
        [StringLength(256)]
        public string mParams;
        
    }
}
