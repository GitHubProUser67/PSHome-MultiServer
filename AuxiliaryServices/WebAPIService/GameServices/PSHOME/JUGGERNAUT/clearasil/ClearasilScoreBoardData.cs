using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class ClearasilScoreBoardData
    : ScoreboardService<ClearasilScoreBoardEntry>
    {
        private string _gameproject;

        public ClearasilScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<ClearasilScoreBoardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<ClearasilScoreBoardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<ClearasilScoreBoardEntry>();
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
                        db.Update(existing);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    await set.AddAsync(new ClearasilScoreBoardEntry
                    {
                        Time = "000",
                        ExtraData1 = _gameproject,
                        PlayerId = playerId,
                        Score = newScore,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task AddTimeAsync(string playerId, string time)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<ClearasilScoreBoardEntry>();
                DateTime now = DateTime.UtcNow;

                var existing = await set
                    .Where(x => x.ExtraData1 == _gameproject)
                    .FirstOrDefaultAsync(e =>
                    e.PsnId != null &&
                    e.PsnId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    existing.Time = time;
                    existing.UpdatedAt = now;
                    db.Update(existing);
                }
                else
                {
                    await set.AddAsync(new ClearasilScoreBoardEntry
                    {
                        PsnId = playerId,
                        ExtraData1 = _gameproject,
                        Time = time,
                        UpdatedAt = now
                    }).ConfigureAwait(false);
                }

                await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("entry",
                    new XElement("user", entry.PsnId),
                    new XElement("score", (int)entry.Score),
                    new XElement("time", entry.Time));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
