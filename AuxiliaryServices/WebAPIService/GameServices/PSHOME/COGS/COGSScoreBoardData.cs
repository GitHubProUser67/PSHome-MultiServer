using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.COGS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.COGS
{
    internal class COGSScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<CogsScoreboardEntry>(options)
    {
        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                xmlScoreboard.Add(
                    new XElement(
                        "player",
                        new XElement("Name", entry.PsnId),
                        new XElement("Points", entry.Score.ToString())
                    )
                );
            }

            return xmlScoreboard.ToString();
        }
    }
}
