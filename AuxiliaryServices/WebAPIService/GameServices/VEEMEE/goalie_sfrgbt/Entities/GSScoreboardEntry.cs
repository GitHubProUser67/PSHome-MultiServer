using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.VEEMEE.goalie_sfrgbt.Entities
{
    public class GSScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string duration { get; set; }
        public string guest { get; set; }
    }
}
