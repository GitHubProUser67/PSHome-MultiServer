using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.JUGGERNAUT.clearasil.Entities
{
    public class ClearasilScoreBoardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string Time = "000";
    }
}
