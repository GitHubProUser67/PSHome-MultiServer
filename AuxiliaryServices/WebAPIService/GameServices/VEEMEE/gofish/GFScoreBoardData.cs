using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace WebAPIService.GameServices.VEEMEE.gofish
{
    public class GFScoreBoardData
    {
        private static DateTime _lastDailyResetTime = DateTime.MinValue;

        private static bool _initiated = false;

        private static object _Lock = new object();
        private static bool _initiatedDaily = false;

        public class ScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
            public string fishcount { get; set; }
            public string biggestfishweight { get; set; }
            public string totalfishweight { get; set; }
        }

        private static List<ScoreboardEntry> scoreboard = new List<ScoreboardEntry>();
        private static List<ScoreboardEntry> scoreboardDaily = new List<ScoreboardEntry>();

        public static void LoadScoreboardFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiated = true;
                return;
            }

            scoreboard.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("player"))
            {
                string psnid = playerElement.Element("psnid")?.Value;
                string scoreStr = playerElement.Element("score")?.Value;
                string fishcount = playerElement.Element("fishcount")?.Value;
                string biggestfishweight = playerElement.Element("biggestfishweight")?.Value;
                string totalfishweight = playerElement.Element("totalfishweight")?.Value;

                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float score);

                scoreboard.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    fishcount = fishcount,
                    biggestfishweight = biggestfishweight,
                    totalfishweight = totalfishweight
                });
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

            _initiated = true;
        }

        public static void LoadScoreboardDailyFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiatedDaily = true;
                return;
            }

            scoreboardDaily.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("player"))
            {
                string psnid = playerElement.Element("psnid")?.Value;
                string scoreStr = playerElement.Element("score")?.Value;
                string fishcount = playerElement.Element("fishcount")?.Value;
                string biggestfishweight = playerElement.Element("biggestfishweight")?.Value;
                string totalfishweight = playerElement.Element("totalfishweight")?.Value;

                float score = 0;
                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out score);

                scoreboardDaily.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    fishcount = fishcount,
                    biggestfishweight = biggestfishweight,
                    totalfishweight = totalfishweight
                });
            }

            scoreboardDaily.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardDaily.Count > 20)
                scoreboardDaily.RemoveRange(20, scoreboardDaily.Count - 20);

            _initiatedDaily = true;
        }

        public static void UpdateScoreBoard(string psnid, string newFishcount, string newBiggestfishweight, string newTotalfishweight, float newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboard.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.fishcount = newFishcount;
                existingEntry.biggestfishweight = newBiggestfishweight;
                existingEntry.totalfishweight = newTotalfishweight;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboard.Count < 20)
                    scoreboard.Add(new ScoreboardEntry { psnid = psnid, score = newScore, fishcount = newFishcount, biggestfishweight = newBiggestfishweight, totalfishweight = newTotalfishweight });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

            UpdateDailyScoreBoard(psnid, newFishcount, newBiggestfishweight, newTotalfishweight, newScore);
        }

        public static void UpdateDailyScoreBoard(string psnid, string newFishcount, string newBiggestfishweight, string newTotalfishweight, float newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboardDaily.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.fishcount = newFishcount;
                existingEntry.biggestfishweight = newBiggestfishweight;
                existingEntry.totalfishweight = newTotalfishweight;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboardDaily.Count < 20)
                    scoreboardDaily.Add(new ScoreboardEntry { psnid = psnid, score = newScore, fishcount = newFishcount, biggestfishweight = newBiggestfishweight, totalfishweight = newTotalfishweight });
            }

            // Sort the scoreboard by score in descending order
            scoreboardDaily.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboardDaily.Count > 20)
                scoreboardDaily.RemoveRange(20, scoreboardDaily.Count - 20);
        }

        public static string ConvertScoreboardToXml(string path)
        {
            if (!_initiated)
                LoadScoreboardFromXml(path);

            XElement xmlScoreboard = new XElement("leaderboard");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public static string ConvertDailyScoreboardToXml(string path)
        {
            if (!_initiatedDaily)
                LoadScoreboardDailyFromXml(path);

            XElement xmlScoreboard = new XElement("leaderboard");

            foreach (var entry in scoreboardDaily)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in scoreboardDaily)
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("fishcount", entry.fishcount ?? "0"),
                    new XElement("biggestfishweight", entry.biggestfishweight ?? "0"),
                    new XElement("totalfishweight", entry.totalfishweight ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public static void UpdateAllTimeScoreboardXml(string apiPath)
        {
            lock (_Lock)
            {
                string filePath = $"{apiPath}/VEEMEE/gofish/leaderboard_alltime.xml";
                Directory.CreateDirectory($"{apiPath}/VEEMEE/gofish");
                File.WriteAllText(filePath, ConvertScoreboardToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - gofish - scoreboard alltime XML updated.");
            }
        }

        private static void CheckAndResetDailyScoreboard()
        {
            DateTime now = DateTime.Now.Date;

            if (_lastDailyResetTime.Date != now)
            {
                scoreboardDaily.Clear();
                _lastDailyResetTime = now;
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - gofish - Daily scoreboard reset at {DateTime.Now}.");
            }
        }

        public static void UpdateTodayScoreboardXml(string apiPath, string date)
        {
            lock (_Lock)
            {
                CheckAndResetDailyScoreboard();
                string filePath = $"{apiPath}/VEEMEE/gofish/leaderboard_{date}.xml";
                Directory.CreateDirectory($"{apiPath}/VEEMEE/gofish");
                File.WriteAllText(filePath, ConvertDailyScoreboardToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - gofish - scoreboard {date} XML updated.");
            }
        }
    }
}
