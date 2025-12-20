using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SetDefaultRosterRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("ROST")]
        [StringLength(30)]
        public string mRosterId;
        
        [TdfMember("TEAM")]
        public uint mTeamId;
        
        [TdfMember("USER")]
        public uint mUserId;
        
    }
}
