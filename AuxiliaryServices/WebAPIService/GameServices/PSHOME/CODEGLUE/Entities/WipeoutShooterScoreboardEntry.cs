using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.CODEGLUE.Entities
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
