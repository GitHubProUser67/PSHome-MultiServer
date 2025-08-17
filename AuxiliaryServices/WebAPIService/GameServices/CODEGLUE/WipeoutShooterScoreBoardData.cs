using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WebAPIService.GameServices.CODEGLUE
{
    internal class WipeoutShooterScoreBoardData
    {
        private static bool _initiated = false;

        private object _Lock = new object();

        public class WipeoutShooterScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
        }

        private List<WipeoutShooterScoreboardEntry> scoreboard = new List<WipeoutShooterScoreboardEntry>();

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
                string psnid = playerElement.Element("NAME")?.Value;
                string scoreStr = playerElement.Element("SCORE")?.Value;

                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float score);

                scoreboard.Add(new WipeoutShooterScoreboardEntry
                {
                    psnid = psnid,
                    score = score
                });
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 10)
                scoreboard.RemoveRange(10, scoreboard.Count - 10);

            _initiated = true;
        }

        public void UpdateScoreBoard(string psnid, float newScore)
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
                if (scoreboard.Count < 10)
                    scoreboard.Add(new WipeoutShooterScoreboardEntry { psnid = psnid, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 10 entries
            if (scoreboard.Count > 10)
                scoreboard.RemoveRange(10, scoreboard.Count - 10);
        }

        private string ConvertScoreboardToXml(string path, string gameName)
        {
            if (!_initiated)
                LoadScoreboardFromXml(path);

            byte i = 1;

            XElement xmlScoreboard = new XElement(gameName);

            foreach (var entry in scoreboard.OrderByDescending(entry => entry.score))
            {
                XElement xmlEntry = new XElement("ENTRY",
                    new XElement("RANK", i),
                    new XElement("NAME", entry.psnid ?? "Voodooperson05"),
                    new XElement("SCORE", entry.score.ToString()));

                xmlScoreboard.Add(xmlEntry);

                i++;
            }

            return xmlScoreboard.ToString();
        }

        public string UpdateScoreboardXml(string apiPath, string gameName)
        {
            string directoryPath = $"{apiPath}/WipeoutShooter/{gameName}";
            string filePath = $"{apiPath}/WipeoutShooter/{gameName}/leaderboard.xml";

            lock (_Lock)
            {
                Directory.CreateDirectory(directoryPath);
                string xmlData = ConvertScoreboardToXml(filePath, gameName);
                File.WriteAllText(filePath, xmlData);
                CustomLogger.LoggerAccessor.LogDebug($"[WipeoutShooter] - {gameName} - scoreboard XML updated.");
                return xmlData;
            }
        }
    }
}
