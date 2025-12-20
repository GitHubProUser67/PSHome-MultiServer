using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora.Entities
{
    public class OrbrunnerScoreBoardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }
    }
}
