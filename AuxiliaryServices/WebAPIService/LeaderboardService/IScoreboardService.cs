using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.LeaderboardService
{
    public interface IScoreboardService<TEntry> where TEntry : ScoreboardEntryBase, new()
    {
        // Top scores
        Task<List<TEntry>> GetTopScoresAsync(int max = 10);
        Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null);
        Task<string> SerializeToString(string gameName, int max = 10);

        // Today scores
        Task<List<TEntry>> GetAllScoresAsync();
        Task<List<TEntry>> GetTodayScoresAsync(int max = 10);
        Task<string> SerializeToDailyString(string gameName, int max = 10);

        // Current week scores
        Task<List<TEntry>> GetCurrentWeekScoresAsync(int max = 10);
        Task<string> SerializeToWeeklyString(string gameName, int max = 10);

        // Current month scores
        Task<List<TEntry>> GetCurrentMonthScoresAsync(int max = 10);
        Task<string> SerializeToMonthlyString(string gameName, int max = 10);
    }

}
