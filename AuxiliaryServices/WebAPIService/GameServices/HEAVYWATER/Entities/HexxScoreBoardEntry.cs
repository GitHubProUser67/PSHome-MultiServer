using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.HEAVYWATER.Entities
{
    public class HexxScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
