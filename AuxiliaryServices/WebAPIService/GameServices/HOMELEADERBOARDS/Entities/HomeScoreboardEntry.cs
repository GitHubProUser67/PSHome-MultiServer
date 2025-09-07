using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.HOMELEADERBOARDS.Entities
{
    public class HomeScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
