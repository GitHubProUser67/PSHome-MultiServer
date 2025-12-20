using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class ClanTeamChallenge
    {
        public int ClanChallengeId { get; set; }
        public int AppId { get; set; }
        public int ChallengerClanID { get; set; }
        public int AgainstClanID { get; set; }
        public int Status { get; set; }
        public int ResponseTime { get; set; }
        public string ChallengeMsg { get; set; }
        public string ResponseMessage { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
    }
}
