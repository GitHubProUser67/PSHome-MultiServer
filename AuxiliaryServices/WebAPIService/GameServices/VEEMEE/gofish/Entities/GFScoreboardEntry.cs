using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.VEEMEE.gofish.Entities
{
    public class GFScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string fishcount { get; set; }
        public string biggestfishweight { get; set; }
        public string totalfishweight { get; set; }
    }
}
