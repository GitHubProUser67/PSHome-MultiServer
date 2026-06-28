using System.Text;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.NDREAMS.Aurora.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    internal class OrbrunnerScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<OrbrunnerScoreBoardEntry>(options)
    {
        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var sb = new StringBuilder();

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
