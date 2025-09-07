using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.VEEMEE.audi_sled.Entities;
using WebAPIService.GameServices.VEEMEE.audi_vrun.Entities;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.audi_vrun
{
    internal class VrunScoreBoardData
   : ScoreboardService<VrunScoreboardEntry>
    {
        public VrunScoreBoardData(LeaderboardDbContext dbContext, object obj = null)
            : base(dbContext)
        {
        }

        public override async Task UpdateScoreAsync(string playerId, float newScore, List<object> extraData = null)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            int numOfRaces = (int)extraData[0];
            float time = (float)extraData[1];

            var set = _dbContext.Set<VrunScoreboardEntry>();
            DateTime now = DateTime.UtcNow; // use UTC for consistency

            var existing = await set
                .FirstOrDefaultAsync(e =>
                e.PlayerId != null &&
                e.PlayerId.ToLower() == playerId.ToLower()).ConfigureAwait(false);

            if (existing != null)
            {
                if (newScore > existing.Score)
                    existing.Score = newScore;

                existing.time = time;
                existing.numOfRaces = numOfRaces;
                existing.UpdatedAt = now; // update timestamp

                _dbContext.Update(existing);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                await set.AddAsync(new VrunScoreboardEntry
                {
                    time = time,
                    numOfRaces = numOfRaces,
                    PlayerId = playerId,
                    Score = newScore,
                    UpdatedAt = now // set timestamp for new entry
                }).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public int GetNumOfRacesForUser(string userName)
        {
            return _dbContext.Set<SledMpScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .Select(x => (int?)x.numOfRaces)
                 .FirstOrDefault() ?? 1;
        }

        public float GetScoreForUser(string userName)
        {
            return _dbContext.Set<VrunScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .Select(x => (float?)x.Score)
                 .FirstOrDefault() ?? (float)0.0;
        }

        public float GetTimeForUser(string userName)
        {
            return _dbContext.Set<VrunScoreboardEntry>()
                 .Where(x => x.PlayerId == userName)
                 .Select(x => (float?)x.time)
                 .FirstOrDefault() ?? (float)0.0;
        }

        public override async Task<string> SerializeToString(string gameName, int max = 8)
        {
            int iY = 142; // Initial Y position
            StringBuilder data = new StringBuilder($"<XML><PAGE><RECT X=\"0\" Y=\"1\" W=\"0\" H=\"0\" col=\"#C0C0C0\" /><RECT X=\"0\" Y=\"0\" W=\"1280\" H=\"720\" col=\"#000000\" /><TEXT X=\"57\" Y=\"42\" col=\"#FFFFFF\" size=\"4\">{gameName}</TEXT>");

            var entries = await GetTopScoresAsync(max);

            for (int i = 0; i < entries.Count; i++)
            {
                data.AppendFormat("<RECT X=\"57\" Y=\"" + iY + "\" W=\"50\" H=\"50\" col=\"#662020\" />");
                data.AppendFormat("<RECT X=\"57\" Y=\"" + (iY + 45) + "\" W=\"50\" H=\"4\" col=\"#873030\" />");
                data.AppendFormat("<RECT X=\"973\" Y=\"" + iY + "\" W=\"254\" H=\"50\" col=\"#662020\" />");
                data.AppendFormat("<RECT X=\"973\" Y=\"" + (iY + 45) + "\" W=\"254\" H=\"4\" col=\"#873030\" />");
                data.AppendFormat("<RECT X=\"107\" Y=\"" + iY + "\" W=\"867\" H=\"50\" col=\"#313131\" />");
                data.AppendFormat("<RECT X=\"107\" Y=\"" + (iY + 45) + "\" W=\"867\" H=\"4\" col=\"#4D4D4D\" />");
                data.AppendFormat("<TEXT X=\"70\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, i + 1);

                data.AppendFormat("<TEXT X=\"190\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, entries[i].PlayerId);
                data.AppendFormat("<TEXT X=\"1015\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1} m</TEXT>", iY + 5, entries[i].Score);

                iY += 46; // Move down for next entry
            }

            data.Append("</PAGE></XML>");

            return data.ToString();
        }
    }
}
