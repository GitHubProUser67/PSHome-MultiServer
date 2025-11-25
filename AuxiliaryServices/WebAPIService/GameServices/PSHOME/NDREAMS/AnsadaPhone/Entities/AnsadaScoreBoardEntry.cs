using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.AnsadaPhone.Entities
{
    public class AnsadaScoreBoardEntry : ScoreboardEntryBase
    {
        public string PsnId
        {
            get => PlayerId;
            set => PlayerId = value;
        }

        public string Time { get; set; }
    }
}
