using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPIService.GameServices.PSHOME.OHS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    internal class OHSScoreBoardData
    : ScoreboardService<OHSScoreboardEntry>
    {
        private string _gameproject;

        public OHSScoreBoardData(DbContextOptions options, object obj = null)
            : base(options)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<OHSScoreboardEntry>> GetAllScoresAsync()
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<OHSScoreboardEntry>()
               .Where(x => x.ExtraData1 == _gameproject)
               .ToListAsync().ConfigureAwait(false);
            }
        }

        public override async Task<List<OHSScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<OHSScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task<List<OHSScoreboardEntry>> GetTopScoresAsyncEx(int start, int count)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                return await db.Set<OHSScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Skip(start - 1) // skip entries before the page
                .Take(count) // take the requested number
                .ToListAsync()
                .ConfigureAwait(false);
            }
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<OHSScoreboardEntry>();
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
                    await set.AddAsync(new OHSScoreboardEntry
                    {
                        ExtraData1 = _gameproject,
                        PlayerId = playerId,
                        Score = newScore,
                        UpdatedAt = now // set timestamp for new entry
                    }).ConfigureAwait(false);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public override async Task<List<OHSScoreboardEntry>> GetTodayScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                if (max == -1)
                    return await db.Set<OHSScoreboardEntry>()
                        .Where(x => x.ExtraData1 == _gameproject)
                        .Where(e => e.UpdatedAt >= today)
                        .OrderByDescending(e => e.Score)
                        .ToListAsync()
                        .ConfigureAwait(false);
                return await db.Set<OHSScoreboardEntry>()
                        .Where(x => x.ExtraData1 == _gameproject)
                        .Where(e => e.UpdatedAt >= today)
                        .OrderByDescending(e => e.Score)
                        .Take(max)
                        .ToListAsync()
                        .ConfigureAwait(false);
            }
        }

        public override async Task<List<OHSScoreboardEntry>> GetCurrentWeekScoresAsync(int max = 10)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var now = DateTime.UtcNow;
                var weekStart = now.Date.AddDays(-((7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7)); // Monday 00:00 UTC
                if (max == -1)
                    return await db.Set<OHSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= weekStart)
                    .OrderByDescending(e => e.Score)
                    .ToListAsync()
                    .ConfigureAwait(false);
                return await db.Set<OHSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= weekStart)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<List<OHSScoreboardEntry>> GetTodayScoresAsyncEx(int start, int count)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                return await db.Set<OHSScoreboardEntry>()
                        .Where(x => x.ExtraData1 == _gameproject)
                        .Where(e => e.UpdatedAt >= today)
                        .OrderByDescending(e => e.Score)
                        .Skip(start - 1) // skip entries before the page
                        .Take(count) // take the requested number
                        .ToListAsync()
                        .ConfigureAwait(false);
            }
        }

        public async Task<List<OHSScoreboardEntry>> GetCurrentWeekScoresAsyncEx(int start, int count)
        {
            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                DateTime today = DateTime.UtcNow.Date;
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = today.AddDays(-1 * diff); // Monday
                return await db.Set<OHSScoreboardEntry>()
                       .Where(x => x.ExtraData1 == _gameproject)
                       .Where(e => e.UpdatedAt >= weekStart)
                       .OrderByDescending(e => e.Score)
                       .Skip(start - 1) // skip entries before the page
                       .Take(count) // take the requested number
                       .ToListAsync()
                       .ConfigureAwait(false);
            }
        }

        public async Task SetJaminExtraData(string playerId, string extraData)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            using (LeaderboardDbContext db = new LeaderboardDbContext(_dboptions))
            {
                db.Database.Migrate();
                var set = db.Set<OHSScoreboardEntry>();
                DateTime now = DateTime.UtcNow; // use UTC for consistency

                var existing = await set
                    .Where(x => x.ExtraData1 == _gameproject)
                    .FirstOrDefaultAsync(e =>
                    e.PlayerId != null &&
                    e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

                if (existing != null)
                {
                    existing.ExtraData2 = extraData;
                    existing.UpdatedAt = now; // update timestamp
                    db.Update(existing);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<string> SerializeToStringEx(string gameName, string user, int max = 20, bool reverse = false)
        {
            int scoreforuser = 0;
            int i = 1;

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

            var entries = await GetTopScoresAsync(max).ConfigureAwait(false);

            if (reverse)
                entries.Reverse();

            foreach (var entry in entries)
            {
                luaTable.Add(i, new Dictionary<string, object>
                {
                    { "[\"user\"]", $"\"{entry.PsnId}\"" },
                    { "[\"score\"]", $"{entry.Score}" }
                });

                if (entry.PsnId == user)
                    scoreforuser = (int)entry.Score;

                i++;
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = {scoreforuser} }}, [\"entries\"] = {FormatScoreBoardLuaTable(luaTable)} }}";
        }

        public async Task<string> SerializeToStringEx(string gameName, string user, int start, int count, bool reverse = false)
        {
            int scoreForUser = 0;
            int i = 1;

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

            var entries = await GetTopScoresAsyncEx(start, count).ConfigureAwait(false);

            if (reverse)
                entries.Reverse();

            foreach (var entry in entries)
            {
                luaTable.Add(i, new Dictionary<string, object>
                {
                    { "[\"user\"]", $"\"{entry.PsnId}\"" },
                    { "[\"score\"]", $"{entry.Score}" }
                });

                if (entry.PsnId == user)
                    scoreForUser = (int)entry.Score;

                i++;
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = {scoreForUser} }}, [\"entries\"] = {FormatScoreBoardLuaTable(luaTable)} }}";
        }

        public async Task<string> SerializeToStringDailyEx(string gameName, string user, int start, int count, bool reverse = false)
        {
            int scoreForUser = 0;
            int i = 1;

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

            var entries = await GetTodayScoresAsyncEx(start, count).ConfigureAwait(false);

            if (reverse)
                entries.Reverse();

            foreach (var entry in entries)
            {
                luaTable.Add(i, new Dictionary<string, object>
                {
                    { "[\"user\"]", $"\"{entry.PsnId}\"" },
                    { "[\"score\"]", $"{entry.Score}" }
                });

                if (entry.PsnId == user)
                    scoreForUser = (int)entry.Score;

                i++;
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = {scoreForUser} }}, [\"entries\"] = {FormatScoreBoardLuaTable(luaTable)} }}";
        }

        public async Task<string> SerializeToWeeklyStringEx(string gameName, string user, int start, int count, bool reverse = false)
        {
            int scoreForUser = 0;
            int i = 1;

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

            var entries = await GetCurrentWeekScoresAsyncEx(start, count).ConfigureAwait(false);

            if (reverse)
                entries.Reverse();

            foreach (var entry in entries)
            {
                luaTable.Add(i, new Dictionary<string, object>
                {
                    { "[\"user\"]", $"\"{entry.PsnId}\"" },
                    { "[\"score\"]", $"{entry.Score}" }
                });

                if (entry.PsnId == user)
                    scoreForUser = (int)entry.Score;

                i++;
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = {scoreForUser} }}, [\"entries\"] = {FormatScoreBoardLuaTable(luaTable)} }}";
        }

        public static string FormatScoreBoardLuaTable(Dictionary<int, Dictionary<string, object>> luaTable)
        {
            string luaString = "{ ";
            foreach (var rankData in luaTable)
            {
                luaString += $"[{rankData.Key}] = {{ ";
                foreach (var kvp in rankData.Value)
                {
                    luaString += $"{kvp.Key} = {kvp.Value}, "; // We already formatted the keys and values accordingly
                }
                luaString = RemoveTrailingComma(luaString); // Remove the trailing comma for the last element in each number category
                luaString += " }, ";
            }
            luaString += " }";

            return RemoveTrailingComma(luaString);
        }

        private static string RemoveTrailingComma(string input)
        {
            return Regex.Replace(input, @",(\s*})|(\s*]\s*})", "$1$2");
        }
    }
}
