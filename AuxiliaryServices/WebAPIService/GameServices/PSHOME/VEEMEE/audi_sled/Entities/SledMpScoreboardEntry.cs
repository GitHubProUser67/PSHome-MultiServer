using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_sled.Entities
{
    public class SledMpScoreboardEntry : ScoreboardEntryBase
    {
        public int numOfRaces { get; set; }
        public float time { get; set; }
    }
}
