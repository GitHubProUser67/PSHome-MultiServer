using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct GetTournamentMemberStatusResponse
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTR")]
        [StringLength(32)]
        public string mTournAttribute;
        
        [TdfMember("BZID")]
        public uint mUserId;
        
        [TdfMember("ISAC")]
        public bool mIsActive;
        
        [TdfMember("LEVL")]
        public uint mLevel;
        
        [TdfMember("LMCH")]
        public uint mLastMatchId;
        
        [TdfMember("TID")]
        public uint mTournamentId;
        
    }
}
