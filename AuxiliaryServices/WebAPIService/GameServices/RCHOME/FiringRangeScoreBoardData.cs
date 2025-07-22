using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WebAPIService.GameServices.RCHOME
{
    internal class FiringRangeScoreBoardData
    {
        private static bool _initiated = false;

        private object _Lock = new object();

        public class FiringRangeScoreboardEntry
        {
            public string psnid { get; set; }
            public int score { get; set; }
        }

        private List<FiringRangeScoreboardEntry> scoreboard = new List<FiringRangeScoreboardEntry>();

        public void LoadScoreboardFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiated = true;
                return;
            }

            scoreboard.Clear();

            foreach (var rowElement in XDocument.Parse(File.ReadAllText(path)).Descendants("row"))
            {
                var cells = rowElement.Elements("c").ToList();

                if (cells.Count == 2)
                {
                    string psnid = cells[0].Value;
                    int.TryParse(cells[1].Value, out int score);

                    scoreboard.Add(new FiringRangeScoreboardEntry
                    {
                        psnid = psnid,
                        score = score
                    });
                }
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 10)
                scoreboard.RemoveRange(10, scoreboard.Count - 10);

            _initiated = true;
        }

        public void UpdateScoreBoard(string psnid, int newScore)
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
                    scoreboard.Add(new FiringRangeScoreboardEntry { psnid = psnid, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 10 entries
            if (scoreboard.Count > 10)
                scoreboard.RemoveRange(10, scoreboard.Count - 10);
        }

        private string ConvertScoreboardToXml(string path)
        {
            if (!_initiated)
                LoadScoreboardFromXml(path);

            XElement xmlScoreboard = new XElement("data");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("row",
                    new XElement("c", entry.psnid ?? "Voodooperson05"),
                    new XElement("c", entry.score.ToString()));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public string UpdateScoreboardXml(string apiPath, string gameName)
        {
            string directoryPath = $"{apiPath}/RCHOME/{gameName}";
            string filePath = $"{apiPath}/RCHOME/{gameName}/leaderboard.xml";

            lock (_Lock)
            {
                Directory.CreateDirectory(directoryPath);
                string xmlData = ConvertScoreboardToXml(filePath);
                File.WriteAllText(filePath, xmlData);
                CustomLogger.LoggerAccessor.LogDebug($"[RCHOME] - {gameName} - scoreboard XML updated.");
                return xmlData;
            }
        }
    }
}
