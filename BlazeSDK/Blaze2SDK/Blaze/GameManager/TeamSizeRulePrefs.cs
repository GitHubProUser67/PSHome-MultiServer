using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct TeamSizeRulePrefs
    {
        
        [TdfMember("SDIF")]
        public ushort mMaxTeamSizeDifferenceAllowed;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLD")]
        [StringLength(32)]
        public string mRangeOffsetListName;
        
        [TdfMember("TID")]
        public ushort mTeamId;
        
        [TdfMember("TMAP")]
        public List<TeamSizeRequirements> mTeamSizeRequirementsVector;
        
    }
}
