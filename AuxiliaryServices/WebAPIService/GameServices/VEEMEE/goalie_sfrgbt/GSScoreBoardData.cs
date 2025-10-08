using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.VEEMEE.goalie_sfrgbt.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.goalie_sfrgbt
{
    internal class GSScoreBoardData
    : ScoreboardService<GSScoreboardEntry>
    {
        private string _gameproject;

        public GSScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<GSScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<GSScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public override async Task<List<GSScoreboardEntry>> GetTodayScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                return await db.Set<GSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= today)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<List<GSScoreboardEntry>> GetYesterdayScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date.AddDays(-1);
                return await db.Set<GSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= today)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public override async Task<List<GSScoreboardEntry>> GetCurrentWeekScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = today.AddDays(-1 * diff); // Monday
                return await db.Set<GSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= weekStart)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public override async Task<List<GSScoreboardEntry>> GetCurrentMonthScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                DateTime monthStart = new DateTime(today.Year, today.Month, 1);
                return await db.Set<GSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= monthStart)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public GSScoreboardEntry GetEntryForUser(string userName)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return db.Set<GSScoreboardEntry>()
                 .Where(x => x.ExtraData1 == _gameproject)
                 .Where(x => x.PlayerId == userName)
                 .FirstOrDefault();
            }
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            string duration = (string)extraData[0];
            string guest = (string)extraData[1];

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<GSScoreboardEntry>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set
                    .Where(x => x.ExtraData1 == _gameproject)
                    .FirstOrDefaultAsync(e =>
                    e.PlayerId != null &&
                    e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    if (newScore > existing.Score)
                        existing.Score = newScore;

                    existing.duration = duration;
                    existing.guest = guest;
                    existing.UpdatedAt = now; // update timestamp

                    db.Update(existing);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    await set.AddAsync(new GSScoreboardEntry
                    {
                        ExtraData1 = _gameproject,
                        duration = duration,
                        guest = guest,
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
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToDailyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTodayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public async Task<string> SerializeToYesterdayString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetYesterdayScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToWeeklyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentWeekScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public override async Task<string> SerializeToMonthlyString(string gameName, int max = 10)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetCurrentMonthScoresAsync(max))
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.PsnId ?? "Voodooperson05"),
                    new XElement("score", entry.Score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
