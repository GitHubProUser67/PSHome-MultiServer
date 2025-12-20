using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct JoinLeagueRequest
    {
        
        [TdfMember("DNF")]
        public int mDNF;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("MMBR")]
        public uint mInviter;
        
        /// <summary>
        /// Max String Length: 13
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(13)]
        public string mPassword;
        
        [TdfMember("TEAM")]
        public uint mTeamId;
        
    }
}
