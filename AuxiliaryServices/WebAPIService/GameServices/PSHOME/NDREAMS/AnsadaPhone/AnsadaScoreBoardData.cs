using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.PSHOME.NDREAMS.AnsadaPhone.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.AnsadaPhone
{
    internal class AnsadaScoreBoardData
    : ScoreboardService<AnsadaScoreBoardEntry>
    {
        private readonly string _gameproject;

        public AnsadaScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<AnsadaScoreBoardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<AnsadaScoreBoardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderBy(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            string time = (string)extraData[0];

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<AnsadaScoreBoardEntry>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set
                    .Where(x => x.ExtraData1 == _gameproject)
                    .FirstOrDefaultAsync(e =>
                    e.PlayerId != null &&
                    e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    if (newScore <= existing.Score)
                    {
                        existing.Score = newScore;
                        existing.Time = time;
                        existing.UpdatedAt = now; // update timestamp
                        db.Update(existing);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    await set.AddAsync(new AnsadaScoreBoardEntry
                    {
                        ExtraData1 = _gameproject,
                        PlayerId = playerId,
                        Score = newScore,
                        Time = time,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            int i = 1;
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("entry",
                    new XElement("position", i),
                    new XElement("name", entry.PsnId),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);

                i++;
            }

            return xmlScoreboard.ToString();
        }
    }
}
