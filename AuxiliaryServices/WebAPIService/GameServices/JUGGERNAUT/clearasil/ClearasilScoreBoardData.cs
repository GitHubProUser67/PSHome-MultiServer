using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPIService.GameServices.JUGGERNAUT.clearasil.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.JUGGERNAUT.clearasil
{
    public class ClearasilScoreBoardData
    : ScoreboardService<ClearasilScoreBoardEntry>
    {
        public ClearasilScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
        }

        public async Task AddTimeAsync(string playerId, string time)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            var set = _dbContext.Set<ClearasilScoreBoardEntry>();
            DateTime now = DateTime.UtcNow;

            var existing = await set.FirstOrDefaultAsync(e =>
                e.PsnId != null &&
                e.PsnId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                existing.Time = time;
                existing.UpdatedAt = now;
                _dbContext.Update(existing);
            }
            else
            {
                await set.AddAsync(new ClearasilScoreBoardEntry
                {
                    PsnId = playerId,
                    Time = time,
                    UpdatedAt = now
                }).ConfigureAwait(false);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public override async Task<string> SerializeToString(string gameName, int max = 20)
        {
            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in await GetTopScoresAsync(max).ConfigureAwait(false))
            {
                XElement xmlEntry = new XElement("entry",
                    new XElement("user", entry.PsnId),
                    new XElement("score", entry.Score),
                    new XElement("time", entry.Time));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }
    }
}
