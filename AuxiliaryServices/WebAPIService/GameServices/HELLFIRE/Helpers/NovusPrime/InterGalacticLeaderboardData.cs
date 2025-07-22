using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebAPIService.GameServices.HELLFIRE.Helpers.NovusPrime
{
    public class InterGalacticLeaderboardData
    {
        private static DateTime _lastResetTime = DateTime.MinValue;
        private static DateTime _lastDailyResetTime = DateTime.MinValue;
        private static DateTime _lastMonthlyResetTime = DateTime.MinValue;

        private static bool _initiatedWeekly = false;
        private static bool _initiatedDaily = false;
        private static bool _initiatedMonthly = false;

        private static object _Lock = new object();

        public class ScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
        }

        private static List<ScoreboardEntry> scoreboardWeekly = new List<ScoreboardEntry>();
        private static List<ScoreboardEntry> scoreboardDaily = new List<ScoreboardEntry>();
        private static List<ScoreboardEntry> scoreboardMonthly = new List<ScoreboardEntry>();

        public static void LoadScoreboardWeeklyFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiatedWeekly = true;
                return;
            }

            scoreboardWeekly.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("player"))
            {
                string psnid = playerElement.Element("DisplayName")?.Value;
                string scoreStr = playerElement.Element("Score")?.Value;

                float.TryParse(scoreStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float score);

                scoreboardWeekly.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score
                });
            }

            scoreboardWeekly.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardWeekly.Count > 20)
                scoreboardWeekly.RemoveRange(20, scoreboardWeekly.Count - 20);

            _initiatedWeekly = true;
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
                string psnid = playerElement.Element("DisplayName")?.Value;
                string scoreStr = playerElement.Element("Score")?.Value;

                float.TryParse(scoreStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float score);

                scoreboardDaily.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score
                });
            }

            scoreboardDaily.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardDaily.Count > 20)
                scoreboardDaily.RemoveRange(20, scoreboardDaily.Count - 20);

            _initiatedDaily = true;
        }

        public static void LoadScoreboardMonthlyFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiatedMonthly = true;
                return;
            }

            scoreboardMonthly.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("player"))
            {
                string psnid = playerElement.Element("DisplayName")?.Value;
                string scoreStr = playerElement.Element("Score")?.Value;

                float.TryParse(scoreStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float score);

                scoreboardMonthly.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score
                });
            }

            scoreboardMonthly.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardMonthly.Count > 20)
                scoreboardMonthly.RemoveRange(20, scoreboardMonthly.Count - 20);

            _initiatedMonthly = true;
        }

        public static void UpdateWeeklyScoreBoard(string psnid, float newScore)
        {
            var existingEntry = scoreboardWeekly.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                if (scoreboardWeekly.Count < 20)
                    scoreboardWeekly.Add(new ScoreboardEntry { psnid = psnid, score = newScore });
            }

            scoreboardWeekly.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardWeekly.Count > 20)
                scoreboardWeekly.RemoveRange(20, scoreboardWeekly.Count - 20);

            UpdateDailyScoreBoard(psnid, newScore);
            UpdateMonthlyScoreBoard(psnid, newScore);
        }

        public static void UpdateDailyScoreBoard(string psnid, float newScore)
        {
            var existingEntry = scoreboardDaily.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                if (scoreboardDaily.Count < 20)
                    scoreboardDaily.Add(new ScoreboardEntry { psnid = psnid, score = newScore });
            }

            scoreboardDaily.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardDaily.Count > 20)
                scoreboardDaily.RemoveRange(20, scoreboardDaily.Count - 20);
        }

        public static void UpdateMonthlyScoreBoard(string psnid, float newScore)
        {
            var existingEntry = scoreboardMonthly.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                if (scoreboardMonthly.Count < 20)
                    scoreboardMonthly.Add(new ScoreboardEntry { psnid = psnid, score = newScore });
            }

            scoreboardMonthly.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardMonthly.Count > 20)
                scoreboardMonthly.RemoveRange(20, scoreboardMonthly.Count - 20);
        }

        public static string ConvertScoreboardWeeklyToXml(string path)
        {
            if (!_initiatedWeekly)
                LoadScoreboardWeeklyFromXml(path);

            XElement xmlScoreboard = new XElement("Response");

            foreach (var entry in scoreboardWeekly)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.psnid ?? "Voodooperson05"),
                    new XElement("Score", entry.score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public static string ConvertScoreboardDailyToXml(string path)
        {
            if (!_initiatedDaily)
                LoadScoreboardDailyFromXml(path);

            XElement xmlScoreboard = new XElement("Response");

            foreach (var entry in scoreboardDaily)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.psnid ?? "Voodooperson05"),
                    new XElement("Score", entry.score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public static string ConvertScoreboardMonthlyToXml(string path)
        {
            if (!_initiatedMonthly)
                LoadScoreboardMonthlyFromXml(path);

            XElement xmlScoreboard = new XElement("Response");

            foreach (var entry in scoreboardMonthly)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("DisplayName", entry.psnid ?? "Voodooperson05"),
                    new XElement("Score", entry.score.ToString().Replace(",", ".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        private static void CheckAndResetScoreboardIfNewWeek()
        {
            DateTime now = DateTime.Now;
            DateTime lastMondayMidnight = now.Date.AddDays(-(int)now.DayOfWeek);

            if (now.DayOfWeek == DayOfWeek.Sunday)
                lastMondayMidnight = lastMondayMidnight.AddDays(-7);

            if (_lastResetTime < lastMondayMidnight)
            {
                scoreboardWeekly.Clear();
                _lastResetTime = now;
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - Global scoreboard reset at {now}.");
            }
        }

        private static void CheckAndResetDailyScoreboard()
        {
            DateTime now = DateTime.Now.Date;

            if (_lastDailyResetTime.Date != now)
            {
                scoreboardDaily.Clear();
                _lastDailyResetTime = now;
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - Daily scoreboard reset at {DateTime.Now}.");
            }
        }

        private static void CheckAndResetMonthlyScoreboard()
        {
            DateTime now = DateTime.Now;

            if (_lastMonthlyResetTime.Month != now.Month || _lastMonthlyResetTime.Year != now.Year)
            {
                scoreboardMonthly.Clear();
                _lastMonthlyResetTime = now;
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - Monthly scoreboard reset at {now}.");
            }
        }

        public static void UpdateTodayScoreboardXml(string apiPath, string date)
        {
            string directoryPath = $"{apiPath}/NovusPrime/galactic_scores";
            string filePath = $"{apiPath}/NovusPrime/galactic_scores/leaderboard_{date}.xml";

            lock (_Lock)
            {
                CheckAndResetDailyScoreboard();
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(filePath, ConvertScoreboardDailyToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - scoreboard {date} XML updated.");
            }
        }

        public static void UpdateWeeklyScoreboardXml(string apiPath, string date)
        {
            lock (_Lock)
            {
                CheckAndResetScoreboardIfNewWeek();

                Directory.CreateDirectory($"{apiPath}/NovusPrime/galactic_scores");

                foreach (string file in Directory.GetFiles($"{apiPath}/NovusPrime/galactic_scores", "leaderboard_weekly_*.xml"))
                {
                    Match match = Regex.Match(file, @"leaderboard_weekly_(\d{4}_\d{2}_\d{2}).xml");
                    if (match.Success)
                    {
                        string fileDate = match.Groups[1].Value;
                        if (DateTime.TryParseExact(fileDate, "yyyy_MM_dd",
                           CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fileDateTime))
                        {
                            if ((DateTime.Parse(date) - fileDateTime).TotalDays <= 7)
                            {
                                File.WriteAllText(file, ConvertScoreboardWeeklyToXml(file));
                                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - Replaced old scoreboard file entry: {file}");
                                return;
                            }
                        }
                    }
                }

                string filePath = $"{apiPath}/NovusPrime/galactic_scores/leaderboard_weekly_{date}.xml";
                File.WriteAllText(filePath, ConvertScoreboardWeeklyToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - scoreboard {date} XML updated.");
            }
        }

        public static void UpdateMonthlyScoreboardXml(string apiPath, string date)
        {
            string directoryPath = $"{apiPath}/NovusPrime/galactic_scores";
            string filePath = $"{apiPath}/NovusPrime/galactic_scores/leaderboard_monthly_{date}.xml";

            lock (_Lock)
            {
                CheckAndResetMonthlyScoreboard();
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(filePath, ConvertScoreboardMonthlyToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[HFGAMES] - NovusPrime - Monthly scoreboard {date} XML updated.");
            }
        }
    }

}
