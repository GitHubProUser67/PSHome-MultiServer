using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.VEEMEE.audi_vrun.Entities
{
    public class VrunScoreboardEntry : ScoreboardEntryBase
    {
        public int numOfRaces { get; set; }
        public float time { get; set; }
    }
}
