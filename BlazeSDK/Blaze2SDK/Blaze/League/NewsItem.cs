using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct NewsItem
    {
        
        [TdfMember("CREA")]
        public LeagueUser mCreator;
        
        [TdfMember("FMT")]
        public NewsFormat mFormat;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NEWS")]
        [StringLength(256)]
        public string mNews;
        
        [TdfMember("NTYP")]
        public NewsMsgType mMsgType;
        
        [TdfMember("NWID")]
        public uint mNewsId;
        
        [TdfMember("PARM")]
        public List<NewsItemParam> mParams;
        
        [TdfMember("TIME")]
        public uint mCreationTime;
        
    }
}
