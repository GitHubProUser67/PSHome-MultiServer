using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.HOMELEADERBOARDS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.HOMELEADERBOARDS
{
    internal class HomeScoreBoardData
    : ScoreboardService<HomeScoreboardEntry>
    {
        private string _gameproject;

        public HomeScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<HomeScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            return await _dbContext.Set<HomeScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var set = _dbContext.Set<HomeScoreboardEntry>();
            DateTime now = DateTime.UtcNow; // use UTC for consistency

            var existing = await set
                .Where(x => x.ExtraData1 == _gameproject)
                .FirstOrDefaultAsync(e =>
                e.PlayerId != null &&
                e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                if (newScore > existing.Score)
                {
                    existing.Score = newScore;
                    existing.UpdatedAt = now; // update timestamp
                    _dbContext.Update(existing);
                    await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            else
            {
                await set.AddAsync(new HomeScoreboardEntry
                {
                    ExtraData1 = _gameproject,
                    PlayerId = playerId,
                    Score = newScore,
                    UpdatedAt = now // set timestamp for new entry
                }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 8)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("ENTRY",
                    new XAttribute("player", entry.PsnId),
                    new XAttribute("score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
