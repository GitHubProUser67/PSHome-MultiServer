using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.NDREAMS.Aurora.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.NDREAMS.Aurora
{
    internal class OrbrunnerScoreBoardData
    : ScoreboardService<OrbrunnerScoreBoardEntry>
    {
        public OrbrunnerScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                if (sb.Length == 0)
                    sb.Append(entry.PsnId + "," + entry.Score);
                else
                    sb.Append("," + entry.PsnId + "," + entry.Score);
            }

            return sb.ToString();
        }
    }
}
