using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.COGS.Entities
{
    public class CogsScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
