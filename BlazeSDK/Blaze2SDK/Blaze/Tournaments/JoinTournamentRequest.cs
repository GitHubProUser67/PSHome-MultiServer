using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct JoinTournamentRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTR")]
        [StringLength(32)]
        public string mTournAttribute;
        
        [TdfMember("TEAM")]
        public int mTeam;
        
        [TdfMember("TNID")]
        public uint mId;
        
    }
}
