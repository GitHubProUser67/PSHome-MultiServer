using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.VEEMEE.gofish.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.gofish
{
    internal class GFScoreBoardData
    : ScoreboardService<GFScoreboardEntry>
    {
        public GFScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
        }

        public async Task<List<GFScoreboardEntry>> GetYesterdayScoresAsync(int max = 20)
        {
            DateTime today = DateTime.UtcNow.Date.AddDays(-1);
            return await _dbContext.Set<GFScoreboardEntry>()
                .Where(e => e.UpdatedAt >= today)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public GFScoreboardEntry GetEntryForUser(string userName)
        {
            return _dbContext.Set<GFScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .FirstOrDefault();
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            string fishcount = (string)extraData[0];
            string biggestfishweight = (string)extraData[1];
            string totalfishweight = (string)extraData[2];

            var set = _dbContext.Set<GFScoreboardEntry>();
            DateTime now = DateTime.UtcNow; // use UTC for consistency

            var existing = await set
                .FirstOrDefaultAsync(e =>
                e.PlayerId != null &&
                e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                if (newScore > existing.Score)
                    existing.Score = newScore;

                existing.fishcount = fishcount;
                existing.biggestfishweight = biggestfishweight;
                existing.totalfishweight = totalfishweight;
                existing.UpdatedAt = now; // update timestamp

                _dbContext.Update(existing);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                await set.AddAsync(new GFScoreboardEntry
                {
                    fishcount = fishcount,
                    biggestfishweight = biggestfishweight,
                    totalfishweight = totalfishweight,
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

            foreach (var entry in await GetTopScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetTopScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

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
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public async Task<string> SerializeToYesterdayString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetYesterdayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in await GetYesterdayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }
    }
}
