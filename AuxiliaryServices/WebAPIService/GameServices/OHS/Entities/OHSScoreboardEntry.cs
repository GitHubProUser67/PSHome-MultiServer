using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.OHS.Entities
{
    public class OHSScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public int WriteKey { get; set; }
    }
}
