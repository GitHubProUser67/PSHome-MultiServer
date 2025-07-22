using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace WebAPIService.GameServices.VEEMEE.olm
{
    public class olmScoreBoardData
    {
        private static DateTime _lastResetTime = DateTime.MinValue;

        private static bool _initiated = false;
        private static bool _initiatedWeekly = false;

        private static object _Lock = new object();

        public class ScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
            public string throws { get; set; }
        }

        private static List<ScoreboardEntry> scoreboard = new List<ScoreboardEntry>();
        private static List<ScoreboardEntry> scoreboardWeekly = new List<ScoreboardEntry>();

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
                string throws = playerElement.Element("throws")?.Value;

                float.TryParse(scoreStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float score);

                scoreboard.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    throws = throws
                });
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

            _initiated = true;
        }

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
                string psnid = playerElement.Element("psnid")?.Value;
                string scoreStr = playerElement.Element("score")?.Value;
                string throws = playerElement.Element("throws")?.Value;

                float score = 0;
                float.TryParse(scoreStr, NumberStyles.Float, CultureInfo.InvariantCulture, out score);

                scoreboardWeekly.Add(new ScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                    throws = throws
                });
            }

            scoreboardWeekly.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboardWeekly.Count > 20)
                scoreboardWeekly.RemoveRange(20, scoreboardWeekly.Count - 20);

            _initiatedWeekly = true;
        }

        public static void UpdateScoreBoard(string psnid, string newthrows, float newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboard.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.throws = newthrows;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboard.Count < 20)
                    scoreboard.Add(new ScoreboardEntry { psnid = psnid, score = newScore, throws = newthrows });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

            UpdateWeeklyScoreBoard(psnid, newthrows, newScore);
        }

        public static void UpdateWeeklyScoreBoard(string psnid, string newthrows, float newScore)
        {
            // Check if the player already exists in the scoreboardWeekly
            var existingEntry = scoreboardWeekly.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                existingEntry.throws = newthrows;

                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboardWeekly, add a new entry
                if (scoreboardWeekly.Count < 20)
                    scoreboardWeekly.Add(new ScoreboardEntry { psnid = psnid, score = newScore, throws = newthrows });
            }

            // Sort the scoreboardWeekly by score in descending order
            scoreboardWeekly.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboardWeekly to the top 20 entries
            if (scoreboardWeekly.Count > 20)
                scoreboardWeekly.RemoveRange(20, scoreboardWeekly.Count - 20);
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
                    new XElement("throws", entry.throws ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public static string ConvertScoreboardWeeklyToXml(string path)
        {
            if (!_initiatedWeekly)
                LoadScoreboardWeeklyFromXml(path);

            XElement xmlScoreboard = new XElement("leaderboard");

            foreach (var entry in scoreboardWeekly)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlScoreboard.Add(xmlEntry);
            }

            XElement xmlGameboard = new XElement("games");

            foreach (var entry in scoreboardWeekly)
            {
                XElement xmlEntry = new XElement("game",
                    new XElement("psnid", entry.psnid ?? "Voodooperson05"),
                    new XElement("score", entry.score.ToString().Replace(",", ".")),
                    new XElement("throws", entry.throws ?? "0"));

                xmlGameboard.Add(xmlEntry);
            }

            xmlScoreboard.Add(xmlGameboard.Elements());

            return xmlScoreboard.ToString();
        }

        public static void UpdateAllTimeScoreboardXml(string apiPath)
        {
            lock (_Lock)
            {
                string filePath = $"{apiPath}/VEEMEE/olm/leaderboard_alltime.xml";
                Directory.CreateDirectory($"{apiPath}/VEEMEE/olm");
                File.WriteAllText(filePath, ConvertScoreboardToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - olm - scoreboard alltime XML updated.");
            }
        }

        private static void CheckAndResetScoreboardIfNewWeek()
        {
            DateTime now = DateTime.Now;
            DateTime lastMondayMidnight = now.Date.AddDays(-(int)now.DayOfWeek);

            if (now.DayOfWeek == DayOfWeek.Sunday)
                lastMondayMidnight = lastMondayMidnight.AddDays(-7); // handle Sunday edge case

            if (_lastResetTime < lastMondayMidnight)
            {
                scoreboardWeekly.Clear();
                _lastResetTime = now;
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - olm - Weekly scoreboard reset at {now}.");
            }
        }

        public static void UpdateWeeklyScoreboardXml(string apiPath, string date)
        {
            lock (_Lock)
            {
                CheckAndResetScoreboardIfNewWeek();

                Directory.CreateDirectory($"{apiPath}/VEEMEE/olm");

                // Get all XML files in the scoreboard folder
                foreach (string file in Directory.GetFiles($"{apiPath}/VEEMEE/olm", "leaderboard_*.xml"))
                {
                    // Extract date from the filename
                    Match match = Regex.Match(file, @"leaderboard_(\d{4}_\d{2}_\d{2}).xml");
                    if (match.Success)
                    {
                        string fileDate = match.Groups[1].Value;

                        // Parse the file date
                        if (DateTime.TryParseExact(fileDate, "yyyy_MM_dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fileDateTime))
                        {
                            // Check if the file is newer than 7 days
                            if ((DateTime.Parse(date) - fileDateTime).TotalDays <= 7)
                            {
                                // Update the older scoreboard.
                                File.WriteAllText(file, ConvertScoreboardWeeklyToXml(file));
                                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - olm - Replaced old scoreboard file entry: {file}");
                                return;
                            }
                        }
                    }
                }

                string filePath = $"{apiPath}/VEEMEE/olm/leaderboard_{date}.xml";
                File.WriteAllText(filePath, ConvertScoreboardWeeklyToXml(filePath));
                CustomLogger.LoggerAccessor.LogDebug($"[VEEMEE] - olm - scoreboard {date} XML updated.");
            }
        }
    }
}
