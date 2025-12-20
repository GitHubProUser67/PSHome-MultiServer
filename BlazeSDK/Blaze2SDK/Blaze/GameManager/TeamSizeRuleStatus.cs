using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct TeamSizeRuleStatus
    {
        
        [TdfMember("TEAM")]
        public List<TeamSizeStatus> mAcceptableTeamSizeVector;
        
    }
}
