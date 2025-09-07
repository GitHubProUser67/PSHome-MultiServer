using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPIService.GameServices.OHS.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.OHS
{
    internal class OHSScoreBoardData
    : ScoreboardService<OHSScoreboardEntry>
    {
        private string _gameproject;

        public OHSScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
            _gameproject = (string)obj;
        }

        public override async Task<List<OHSScoreboardEntry>> GetAllScoresAsync()
        {
            return await _dbContext.Set<OHSScoreboardEntry>()
               .Where(x => x.ExtraData1 == _gameproject)
               .ToListAsync().ConfigureAwait(false);
        }

        public override async Task<List<OHSScoreboardEntry>> GetTopScoresAsync(int max = 10)
        {
            return await _dbContext.Set<OHSScoreboardEntry>()
                .Where(x => x.ExtraData1 == _gameproject)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var set = _dbContext.Set<OHSScoreboardEntry>();
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
                await set.AddAsync(new OHSScoreboardEntry
                {
                    ExtraData1 = _gameproject,
                    PlayerId = playerId,
                    Score = newScore,
                    UpdatedAt = now // set timestamp for new entry
                }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public override async Task<List<OHSScoreboardEntry>> GetTodayScoresAsync(int max = 10)
        {
            DateTime today = DateTime.UtcNow.Date;
            if (max == -1)
                return await _dbContext.Set<OHSScoreboardEntry>()
                        .Where(x => x.ExtraData1 == _gameproject)
                        .Where(e => e.UpdatedAt >= today)
                        .OrderByDescending(e => e.Score)
                        .ToListAsync()
                        .ConfigureAwait(false);
            else
                return await _dbContext.Set<OHSScoreboardEntry>()
                    .Where(x => x.ExtraData1 == _gameproject)
                    .Where(e => e.UpdatedAt >= today)
                    .OrderByDescending(e => e.Score)
                    .Take(max)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task SetJaminExtraData(string playerId, string extraData)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var set = _dbContext.Set<OHSScoreboardEntry>();
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
                _dbContext.Update(existing);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<string> SerializeToStringEx(string gameName, string user, int max = 20)
        {
            int scoreforuser = 0;
            int i = 1;

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
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

        public async Task<string> SerializeToStringEx(string gameName, string user, int start, int count)
        {
            int scoreForUser = 0;
            int i = 1;

            // Skip "start" entries and take "count" after that
            var entries = (await GetTopScoresAsync(start + count).ConfigureAwait(false))
                          .Skip(start)
                          .Take(count)
                          .ToList();

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

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

        public async Task<string> SerializeToStringDailyEx(string gameName, string user, int start, int count)
        {
            int scoreForUser = 0;
            int i = 1;

            // Skip "start" entries and take "count" after that
            var entries = (await GetTodayScoresAsync(start + count).ConfigureAwait(false))
                          .Skip(start)
                          .Take(count)
                          .ToList();

            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

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
