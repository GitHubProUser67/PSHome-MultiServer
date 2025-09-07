using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.VEEMEE.olm.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.olm
{
    internal class OLMScoreBoardData
    : ScoreboardService<OLMScoreboardEntry>
    {
        public OLMScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
        }

        public OLMScoreboardEntry GetEntryForUser(string userName)
        {
            return _dbContext.Set<OLMScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .FirstOrDefault();
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            string throws = (string)extraData[0];

            var set = _dbContext.Set<OLMScoreboardEntry>();
            DateTime now = DateTime.UtcNow; // use UTC for consistency

            var existing = await set
                .FirstOrDefaultAsync(e =>
                e.PlayerId != null &&
                e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                if (newScore > existing.Score)
                    existing.Score = newScore;

                existing.throws = throws;
                existing.UpdatedAt = now; // update timestamp

                _dbContext.Update(existing);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                await set.AddAsync(new OLMScoreboardEntry
                {
                    throws = throws,
                    PlayerId = playerId,
                    Score = newScore,
                    UpdatedAt = now // set timestamp for new entry
                }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToDailyString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToWeeklyString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentWeekScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetCurrentWeekScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }
    }
}
