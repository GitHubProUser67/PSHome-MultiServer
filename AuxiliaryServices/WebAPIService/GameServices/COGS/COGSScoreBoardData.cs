using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace WebAPIService.GameServices.COGS
{
    internal class COGSScoreBoardData
    {
        private static bool _initiated = false;

        private object _Lock = new object();

        public class COGSScoreboardEntry
        {
            public string psnid { get; set; }
            public float score { get; set; }
        }

        private List<COGSScoreboardEntry> scoreboard = new List<COGSScoreboardEntry>();

        public void LoadScoreboardFromXml(string path)
        {
            if (!File.Exists(path))
            {
                _initiated = true;
                return;
            }

            scoreboard.Clear();

            foreach (var playerElement in XDocument.Parse(File.ReadAllText(path)).Descendants("player"))
            {
                string psnid = playerElement.Element("Name")?.Value;
                string scoreStr = playerElement.Element("Points")?.Value;

                float.TryParse(scoreStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float score);

                scoreboard.Add(new COGSScoreboardEntry
                {
                    psnid = psnid,
                    score = score
                });
            }

            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);

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
                if (scoreboard.Count < 20)
                    scoreboard.Add(new COGSScoreboardEntry { psnid = psnid, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);
        }

        private string ConvertScoreboardToXml(string path)
        {
            if (!_initiated)
                LoadScoreboardFromXml(path);

            XElement xmlScoreboard = new XElement("xml");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("player",
                    new XElement("Name", entry.psnid ?? "Voodooperson05"),
                    new XElement("Points", entry.score.ToString()));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public string UpdateScoreboardXml(string apiPath)
        {
            string directoryPath = $"{apiPath}/COGS";
            string filePath = $"{apiPath}/COGS/leaderboard.xml";

            lock (_Lock)
            {
                Directory.CreateDirectory(directoryPath);
                string xmlData = ConvertScoreboardToXml(filePath);
                File.WriteAllText(filePath, xmlData);
                CustomLogger.LoggerAccessor.LogDebug($"[COGS] - scoreboard XML updated.");
                return xmlData;
            }
        }
    }
}
