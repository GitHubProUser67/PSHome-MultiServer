using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.NovusPrime;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.NovusPrime
{
    public class InterGalacticScoreBoardData
   : ScoreboardService<InterGalacticScoreboardEntry>
    {
        public InterGalacticScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToDailyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToWeeklyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentWeekScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToMonthlyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentMonthScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.PsnId),
                    new XElement("Score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
