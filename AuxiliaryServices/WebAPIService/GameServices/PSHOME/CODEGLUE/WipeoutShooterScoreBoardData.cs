using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.PSHOME.CODEGLUE.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.CODEGLUE
{
    internal class WipeoutShooterScoreBoardData
    : ScoreboardService<WipeoutShooterScoreboardEntry>
    {
        private string _gametype;

        public WipeoutShooterScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
            _gametype = (string)obj;
        }

        public override async Task<List<WipeoutShooterScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<WipeoutShooterScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gametype)
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
                var set = db.Set<WipeoutShooterScoreboardEntry>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set
                    .Where(x => x.ExtraData1 == _gametype)
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
                    await set.AddAsync(new WipeoutShooterScoreboardEntry
                    {
                        ExtraData1 = _gametype,
                        PlayerId = playerId,
                        Score = newScore,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var entries = await GetTopScoresAsync(max).ConfigureAwait(false);

            byte i = 1;

            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                xmlScoreboard.Add(new XElement("ENTRY",
                    new XElement("RANK", i),
                    new XElement("NAME", entry.PsnId),
                    new XElement("SCORE", entry.Score.ToString())));

                i++;
            }

            return xmlScoreboard.ToString();
        }
    }
}
