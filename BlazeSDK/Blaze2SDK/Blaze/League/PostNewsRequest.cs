using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct PostNewsRequest
    {
        
        [TdfMember("FRMT")]
        public NewsFormat mFormat;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NEWS")]
        [StringLength(256)]
        public string mNews;
        
        [TdfMember("NTYP")]
        public NewsMsgType mMsgType;
        
    }
}
