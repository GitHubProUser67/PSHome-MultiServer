using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.PSHOME.NDREAMS.Espionage9.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Espionage9
{
    internal class Espionage9ScoreBoardData
    : ScoreboardService<Espionage9ScoreBoardEntity>
    {
        public Espionage9ScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            int i = 1;
            StringBuilder sb = new StringBuilder("<xml><success>true</success>");

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                sb.Append($"<high name=\"{entry.PsnId}\" pos=\"{i}\" score=\"{(int)entry.Score}\"/>");
                i++;
            }

            sb.Append("</xml>");

            return sb.ToString();
        }
    }
}
