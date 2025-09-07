using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.CODEGLUE.Entities
{
    public class WipeoutShooterScoreboardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
