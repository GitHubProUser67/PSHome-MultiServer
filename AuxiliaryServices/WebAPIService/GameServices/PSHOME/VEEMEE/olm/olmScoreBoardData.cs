using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.VEEMEE.olm.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.olm
{
    internal class OLMScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<OLMScoreboardEntry>(options)
    {
        public OLMScoreboardEntry GetEntryForUser(string userName)
        {
            using (var db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return db.Set<OLMScoreboardEntry>()
                    .Where(x => x.PlayerId == userName)
                    .FirstOrDefault();
            }
        }

        public override async Task UpdateScoreAsync(
            string playerId,
            float newScore,
            List<object> extraData = null
        )
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var throws = (string)extraData[0];

            using (var db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<OLMScoreboardEntry>();
                var now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set.FirstOrDefaultAsync(e =>
                        e.PlayerId != null && e.PlayerId.ToLower() == playerId.ToLower()
                    )
                    .ConfigureAwait(false);

                if (existing != null)
                {
                    if (newScore > existing.Score)
                        existing.Score = newScore;

                    existing.throws = throws;
                    existing.UpdatedAt = now; // update timestamp

                    db.Update(existing);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    await set.AddAsync(
                            new OLMScoreboardEntry
                            {
                                throws = throws,
                                PlayerId = playerId,
                                Score = newScore,
                                UpdatedAt = now, // set timestamp for new entry
                            }
                        )
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 20)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlScoreboard.Add(xmlEntry);
            }

            var xmlGameboard = new XElement("games");

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToDailyString(string gameName, int max = 20)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlScoreboard.Add(xmlEntry);
            }

            var xmlGameboard = new XElement("games");

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToWeeklyString(string gameName, int max = 20)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentWeekScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlScoreboard.Add(xmlEntry);
            }

            var xmlGameboard = new XElement("games");

            foreach (var entry in await GetCurrentWeekScoresAsync(max))
            {
                var xmlEntry = new XElement(
                    "game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0")
                );

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }
    }
}
