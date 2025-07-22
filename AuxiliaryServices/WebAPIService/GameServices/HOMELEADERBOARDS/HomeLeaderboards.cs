using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace WebAPIService.GameServices.HOMELEADERBOARDS
{
    internal class HomeLeaderboards
    {
        private static bool _initiated = false;

        private object _Lock = new object();

        public class HomeScoreboardEntry
        {
            public string psnid { get; set; }
            public double score { get; set; }
        }

        private List<HomeScoreboardEntry> scoreboard = new List<HomeScoreboardEntry>();

        public void LoadScoreboardFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiated = true;
                return;
            }

            scoreboard.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("ENTRY"))
            {
                string psnid = playerElement.Attribute("player")?.Value;
                string scoreStr = playerElement.Attribute("score")?.Value;

                double.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double score);

                scoreboard.Add(new HomeScoreboardEntry
                {
                    psnid = psnid,
                    score = score,
                });
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 8)
                scoreboard.RemoveRange(8, scoreboard.Count - 8);

            _initiated = true;
        }

        public void UpdateScoreBoard(string psnid, double newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboard.Find(e => e.psnid != null && e.psnid.Equals(psnid, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
            {
                // If the new score is higher, update the existing entry
                if (newScore > existingEntry.score)
                    existingEntry.score = newScore;
            }
            else
            {
                // If the player is not in the scoreboard, add a new entry
                if (scoreboard.Count < 8)
                    scoreboard.Add(new HomeScoreboardEntry { psnid = psnid, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 8 entries
            if (scoreboard.Count > 8)
                scoreboard.RemoveRange(8, scoreboard.Count - 8);
        }

        private string ConvertScoreboardToXml(string path)
        {
            if (!_initiated)
                LoadScoreboardFromXml(path);

            XElement xmlScoreboard = new XElement("PAGE");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("ENTRY",
                    new XAttribute("player", entry.psnid ?? "Voodooperson05"),
                    new XAttribute("score", ((float)entry.score).ToString().Replace(",",".")));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public string UpdateScoreboardXml(string apiPath, string gameName)
        {
            string directoryPath = $"{apiPath}/HOME_LEADERBOARDS/{gameName}";
            string filePath = $"{apiPath}/HOME_LEADERBOARDS/{gameName}/leaderboard.xml";

            lock (_Lock)
            {
                Directory.CreateDirectory(directoryPath);
                string xmlData = ConvertScoreboardToXml(filePath);
                File.WriteAllText(filePath, xmlData);
                CustomLogger.LoggerAccessor.LogDebug($"[HOMELEADERBOARDS] - {gameName} - scoreboard XML updated.");
                return xmlData;
            }
        }
    }
}
