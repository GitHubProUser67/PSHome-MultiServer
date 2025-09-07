using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.LeaderboardService
{
    public class ScoreboardService<TEntry> : IScoreboardService<TEntry>
    where TEntry : ScoreboardEntryBase, new()
    {
        protected readonly LeaderboardDbContext _dbContext;

        public ScoreboardService(LeaderboardDbContext dbContext, object obj = null)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<List<TEntry>> GetAllScoresAsync()
        {
            return await _dbContext.Set<TEntry>()
                .ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<List<TEntry>> GetTopScoresAsync(int max = 10)
        {
            return await _dbContext.Set<TEntry>()
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<List<TEntry>> GetTodayScoresAsync(int max = 10)
        {
            DateTime today = DateTime.UtcNow.Date;
            return await _dbContext.Set<TEntry>()
                .Where(e => e.UpdatedAt >= today)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public virtual async Task<List<TEntry>> GetCurrentWeekScoresAsync(int max = 10)
        {
            DateTime today = DateTime.UtcNow.Date;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = today.AddDays(-1 * diff); // Monday
            return await _dbContext.Set<TEntry>()
                .Where(e => e.UpdatedAt >= weekStart)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public virtual async Task<List<TEntry>> GetCurrentMonthScoresAsync(int max = 10)
        {
            DateTime today = DateTime.UtcNow.Date;
            DateTime monthStart = new DateTime(today.Year, today.Month, 1);
            return await _dbContext.Set<TEntry>()
                .Where(e => e.UpdatedAt >= monthStart)
                .OrderByDescending(e => e.Score)
                .Take(max)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public virtual async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var set = _dbContext.Set<TEntry>();
            DateTime now = DateTime.UtcNow; // use UTC for consistency

            var existing = await set.FirstOrDefaultAsync(e =>
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
                await set.AddAsync(new TEntry
                {
                    PlayerId = playerId,
                    Score = newScore,
                    UpdatedAt = now // set timestamp for new entry
                }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Serialize the leaderboard entries to a string.
        /// Default implementation returns a simple CSV.
        /// Override for custom formats like JSON or XML.
        /// </summary>
        public virtual async Task<string> SerializeToString(string gameName, int max = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine(gameName + ":Rank,PlayerId,Score");

            int rank = 1;
            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                sb.AppendLine($"{rank},{entry.PlayerId},{entry.Score}");
                rank++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serialize the leaderboard entries to a daily string.
        /// Default implementation returns a simple CSV.
        /// Override for custom formats like JSON or XML.
        /// </summary>
        public virtual async Task<string> SerializeToDailyString(string gameName, int max = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine(gameName + ":Rank,PlayerId,Score");

            int rank = 1;
            foreach (var entry in await GetTodayScoresAsync(max).ConfigureAwait(false))
            {
                sb.AppendLine($"{rank},{entry.PlayerId},{entry.Score}");
                rank++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serialize the leaderboard entries to a weekly string.
        /// Default implementation returns a simple CSV.
        /// Override for custom formats like JSON or XML.
        /// </summary>
        public virtual async Task<string> SerializeToWeeklyString(string gameName, int max = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine(gameName + ":Rank,PlayerId,Score");

            int rank = 1;
            foreach (var entry in await GetCurrentWeekScoresAsync(max).ConfigureAwait(false))
            {
                sb.AppendLine($"{rank},{entry.PlayerId},{entry.Score}");
                rank++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serialize the leaderboard entries to a monthly string.
        /// Default implementation returns a simple CSV.
        /// Override for custom formats like JSON or XML.
        /// </summary>
        public virtual async Task<string> SerializeToMonthlyString(string gameName, int max = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine(gameName + ":Rank,PlayerId,Score");

            int rank = 1;
            foreach (var entry in await GetCurrentMonthScoresAsync(max).ConfigureAwait(false))
            {
                sb.AppendLine($"{rank},{entry.PlayerId},{entry.Score}");
                rank++;
            }

            return sb.ToString();
        }
    }
}
