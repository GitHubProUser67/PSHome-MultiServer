using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.NovusPrime;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.NovusPrime
{
    public class InterGalacticScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<InterGalacticScoreboardEntry>(options)
    {
        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", "."))
                );

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToDailyString(string gameName, int max = 10)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max).ConfigureAwait(false))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", "."))
                );

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToWeeklyString(string gameName, int max = 10)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentWeekScoresAsync(max).ConfigureAwait(false))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", "."))
                );

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToMonthlyString(string gameName, int max = 10)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentMonthScoresAsync(max).ConfigureAwait(false))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", "."))
                );

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
