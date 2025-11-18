using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.PSHOME.NDREAMS.Xi2.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2
{
    internal class BattleContScoreBoardData
    : ScoreboardService<BattleContScoreBoardEntity>
    {
        public BattleContScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
        }

        public virtual async Task UpdateWinsAsync(string psnId, int newWins)
        {
            if (string.IsNullOrEmpty(psnId))
                return;

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<BattleContScoreBoardEntity>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set.FirstOrDefaultAsync(e =>
                    e.PsnId != null &&
                    e.PsnId.ToLower() == psnId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    existing.Wins = newWins;
                    existing.UpdatedAt = now; // update timestamp
                    db.Update(existing);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    await set.AddAsync(new BattleContScoreBoardEntity
                    {
                        PlayerId = psnId,
                        Wins = newWins,
                        Score = 0,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task<string> SerializeToString(string gameName, int max = 10)
        {
            int i = 1;
            StringBuilder sb = new StringBuilder("<xml><success>true</success><result><Success>true</Success>");

            var entries = await GetTopScoresAsync(max).ConfigureAwait(false);

            foreach (var entry in entries)
            {
                sb.Append($"<Scores name=\"{entry.PsnId}\" rank=\"{i}\" score=\"{(int)entry.Score}\"/>");
                i++;
            }

            i = 1;

            foreach (var entry in entries)
            {
                sb.Append($"<Wins name=\"{entry.PsnId}\" rank=\"{i}\" wins=\"{entry.Wins}\"/>");
                i++;
            }

            sb.Append("</result></xml>");

            return sb.ToString();
        }
    }
}
