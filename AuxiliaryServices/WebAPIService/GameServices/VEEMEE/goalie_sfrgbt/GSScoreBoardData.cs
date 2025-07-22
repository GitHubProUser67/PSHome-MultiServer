using System.IO;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace WebAPIService.GameServices.VEEMEE.goalie_sfrgbt
{
    public class GSScoreBoardData
    {
        private static DateTime _lastDailyResetTime = DateTime.MinValue;

        private static bool _initiated = false;

        private static object _Lock = new object();
        private static bool _initiatedDaily = false;

        public class ScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
            public string duration { get; set; }
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
                string duration = playerElement.Element("duration")?.Value;

                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float score);

                scoreboard.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    duration = duration
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
                string duration = playerElement.Element("duration")?.Value;

                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float score);

                scoreboardDaily.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    duration = duration
                });
            }

            scoreboardDaily.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardDaily.Count > 20)
                scoreboardDaily.RemoveRange(20, scoreboardDaily.Count - 20);

            _initiatedDaily = true;
        }

        public static void UpdateScoreBoard(string psnid, string newDuration, float newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboard.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.duration = newDuration;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboard.Count < 20)
                    scoreboard.Add(new ScoreboardEntry { psnid = psnid, score = newScore, duration = newDuration });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

            UpdateDailyScoreBoard(psnid, newDuration, newScore);
        }

        public static void UpdateDailyScoreBoard(string psnid, string newDuration, float newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboardDaily.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.duration = newDuration;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboardDaily.Count < 20)
                    scoreboardDaily.Add(new ScoreboardEntry { psnid = psnid, score = newScore, duration = newDuration });
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
                    new XElement("score", entry.score.ToString().Replace(",",".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public static string ConvertScoreboardDailyToXml(string path)
        {
            if (!_initiatedDaily)
                LoadScoreboardDailyFromXml(path);

            XElement xmlScoreboard = new XElement("leaderboard");

            foreach (var entry in scoreboardDaily)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("duration", entry.duration ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public static void UpdateAllTimeScoreboardXml(string apiPath, bool global)
        {
            string directoryPath = string.Empty;
            string filePath = string.Empty;

            if (global)
            {
                directoryPath = $"{apiPath}/VEEMEE/goalie";
                filePath = $"{apiPath}/VEEMEE/goalie/leaderboard_alltime.xml";
            }
            else
            {
                directoryPath = $"{apiPath}/VEEMEE/sfrgbt";
                filePath = $"{apiPath}/VEEMEE/sfrgbt/leaderboard_alltime.xml";
            }

            lock (_Lock)
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(filePath, ConvertScoreboardToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - goalie_sfrgbt - scoreboard alltime XML updated.");
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

        public static void UpdateTodayScoreboardXml(string apiPath, bool global, string date)
        {
            string directoryPath = string.Empty;
            string filePath = string.Empty;

            if (global)
            {
                directoryPath = $"{apiPath}/VEEMEE/goalie";
                filePath = $"{apiPath}/VEEMEE/goalie/leaderboard_{date}.xml";
            }
            else
            {
                directoryPath = $"{apiPath}/VEEMEE/sfrgbt";
                filePath = $"{apiPath}/VEEMEE/sfrgbt/leaderboard_{date}.xml";
            }

            lock (_Lock)
            {
                CheckAndResetDailyScoreboard();
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(filePath, ConvertScoreboardDailyToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - goalie_sfrgbt - scoreboard {date} XML updated.");
            }
        }
    }
}
