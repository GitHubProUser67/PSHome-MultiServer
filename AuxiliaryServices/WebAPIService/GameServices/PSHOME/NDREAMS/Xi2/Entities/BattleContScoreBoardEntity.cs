using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2.Entities
{
    public class BattleContScoreBoardEntity : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public int Wins { get; set; }
    }
}
