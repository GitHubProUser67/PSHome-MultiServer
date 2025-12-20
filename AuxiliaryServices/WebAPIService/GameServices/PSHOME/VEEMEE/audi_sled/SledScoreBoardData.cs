using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.PSHOME.VEEMEE.audi_sled.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_sled
{
    internal class SledScoreBoardData
   : ScoreboardService<SledScoreboardEntry>
    {
        public SledScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
        }

        public override async Task<List<SledScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<SledScoreboardEntry>()
                .OrderBy(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            int numOfRaces = (int)extraData[0];

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<SledScoreboardEntry>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set
                    .FirstOrDefaultAsync(e =>
                    e.PlayerId != null &&
                    e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    if (newScore < existing.Score)
                        existing.Score = newScore;

                    existing.numOfRaces = numOfRaces;
                    existing.UpdatedAt = now; // update timestamp

                    db.Update(existing);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    await set.AddAsync(new SledScoreboardEntry
                    {
                        numOfRaces = numOfRaces,
                        PlayerId = playerId,
                        Score = newScore,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public int GetNumOfRacesForUser(string userName)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return db.Set<SledScoreboardEntry>()
                .Where(x => x.PlayerId == userName)
                .Select(x => (int?)x.numOfRaces)
                .FirstOrDefault() ?? 1;
            }
        }

        public float GetScoreForUser(string userName)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return db.Set<SledScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .Select(x => (float?)x.Score)
                 .FirstOrDefault() ?? (float)0.0;
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            int iY = 142; // Initial Y position
            StringBuilder data = new StringBuilder($"<XML><PAGE><TEXT X=\"100\" Y=\"70\" col=\"#FFFFFF\" size=\"4\">{gameName}</TEXT>");

            var entries = await GetTopScoresAsync(max);

            for (int i = 0; i < entries.Count; i++)
            {
                data.AppendFormat("<TEXT X=\"100\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 7, i + 1);
                data.AppendFormat("<TEXT X=\"190\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, entries[i].PlayerId);
                data.AppendFormat("<TEXT X=\"800\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, entries[i].numOfRaces);
                data.AppendFormat("<TEXT X=\"1060\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, AudiSledSecondsAsString(entries[i].Score));

                iY += 46; // Move down for next entry
            }

            data.Append("</PAGE></XML>");

            return data.ToString();
        }

        private static string AudiSledSecondsAsString(float time)
        {
            if (time < float.Epsilon)
                return " -- : -- . --";

            int seconds = (int)Math.Floor(time);
            if (seconds < 0)
                seconds = 0;

            int hundreds = (int)Math.Floor((time - seconds) * 100 + 0.5);
            if (hundreds < 0)
                hundreds = 0;

            int minutes = seconds / 60;
            seconds %= 60;

            return string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, hundreds);
        }
    }
}
