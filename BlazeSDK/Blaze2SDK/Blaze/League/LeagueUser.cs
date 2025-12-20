using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct LeagueUser
    {
        
        [TdfMember("BLID")]
        public uint mBlazeId;
        
        /// <summary>
        /// Max String Length: 20
        /// </summary>
        [TdfMember("PERS")]
        [StringLength(20)]
        public string mPersona;
        
    }
}
