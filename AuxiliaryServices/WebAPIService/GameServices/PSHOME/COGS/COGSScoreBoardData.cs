using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.PSHOME.COGS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.COGS
{
    internal class COGSScoreBoardData
    : ScoreboardService<CogsScoreboardEntry>
    {
        public COGSScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                xmlScoreboard.Add(new XElement("player",
                    new XElement("Name", entry.PsnId),
                    new XElement("Points", entry.Score.ToString())));
            }

            return xmlScoreboard.ToString();
        }
    }
}
