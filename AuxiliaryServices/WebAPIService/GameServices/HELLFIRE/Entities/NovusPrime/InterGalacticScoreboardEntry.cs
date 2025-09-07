using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.HELLFIRE.Entities.NovusPrime
{
    public class InterGalacticScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
