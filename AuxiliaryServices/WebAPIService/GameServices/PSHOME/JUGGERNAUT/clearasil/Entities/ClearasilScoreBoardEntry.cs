using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil.Entities
{
    public class ClearasilScoreBoardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string Time { get; set; }
    }
}
