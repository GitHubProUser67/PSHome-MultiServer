using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct TournamentTrophyData
    {
        
        [TdfMember("BZID")]
        public uint mUserId;
        
        [TdfMember("CONT")]
        public uint mAwardCount;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("META")]
        [StringLength(64)]
        public string mTrophyMetaData;
        
        [TdfMember("TIME")]
        public uint mAwardTime;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TLOC")]
        [StringLength(32)]
        public string mTrophyNameLocKey;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TNAM")]
        [StringLength(32)]
        public string mTrophyName;
        
        [TdfMember("TNID")]
        public uint mTournamentId;
        
    }
}
