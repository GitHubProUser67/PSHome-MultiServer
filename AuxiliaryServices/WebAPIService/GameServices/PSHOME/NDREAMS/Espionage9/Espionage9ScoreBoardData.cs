using System.Text;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.NDREAMS.Espionage9.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Espionage9
{
    internal class Espionage9ScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<Espionage9ScoreBoardEntity>(options)
    {
        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var i = 1;
            var sb = new StringBuilder("<xml><success>true</success>");

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                sb.Append(
                    $"<high name=\"{entry.PsnId}\" pos=\"{i}\" score=\"{(int)entry.Score}\"/>"
                );
                i++;
            }

            sb.Append("</xml>");

            return sb.ToString();
        }
    }
}
