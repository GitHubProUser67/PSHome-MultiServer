using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WebAPIService.GameServices.PSHOME.HOMELEADERBOARDS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HOMELEADERBOARDS
{
    internal class HomeScoreBoardData(
        DbContextOptions<LeaderboardDbContext> options,
        object obj = null
    ) : ScoreboardService<HomeScoreboardEntry>(options)
    {
        private readonly string _gameproject = (string)obj;

        public override async Task<List<HomeScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (var db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<HomeScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
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

            using (var db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<HomeScoreboardEntry>();
                var now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set.Where(x => x.ExtraData1 == _gameproject)
                    .FirstOrDefaultAsync(e =>
                        e.PlayerId != null && e.PlayerId.ToLower() == playerId.ToLower()
                    )
                    .ConfigureAwait(false);

                if (existing != null)
                {
                    if (newScore > existing.Score)
                    {
                        existing.Score = newScore;
                        existing.UpdatedAt = now; // update timestamp
                        db.Update(existing);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    await set.AddAsync(
                            new HomeScoreboardEntry
                            {
                                ExtraData1 = _gameproject,
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

        public override async Task<string> SerializeToString(string gameName, int max = 8)
        {
            var xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                var xmlEntry = new XElement(
                    "ENTRY",
                    new XAttribute("player", entry.PsnId),
                    new XAttribute("score", entry.Score.ToString().Replace(",", "."))
                );

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
