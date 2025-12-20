using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.olm.Entities
{
    public class OLMScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string throws { get; set; }
    }
}
