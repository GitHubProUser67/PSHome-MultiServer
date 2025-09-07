using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.RCHOME.Entities
{
    public class FiringRangeScoreBoardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
