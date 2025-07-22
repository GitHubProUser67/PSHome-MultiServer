using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace WebAPIService.GameServices.JUGGERNAUT.clearasil
{
    public class ScoreBoardData
    {
        public class ScoreboardEntry
        {
            public string name { get; set; }
            public int score { get; set; }
            public string time { get; set; }
        }

        private static List<ScoreboardEntry> scoreboard = new List<ScoreboardEntry>();

        public static void UpdateScore(string name, int newScore)
        {
            // Check if the player already exists in the scoreboard
            var existingEntry = scoreboard.Find(e => e.name!= null && e.name.Equals(name, StringComparison.OrdinalIgnoreCase));

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
                    scoreboard.Add(new ScoreboardEntry { name = name, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 20 entries
            if (scoreboard.Count > 20)
                scoreboard.RemoveRange(20, scoreboard.Count - 20);
        }

        public static void UpdateTime(string name, string newTime)
        {
            var existingEntry = scoreboard.Find(e => e.name != null && e.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existingEntry != null)
                existingEntry.time = newTime;
        }

        public static string ConvertScoreboardToXml()
        {
            XElement xmlScoreboard = new XElement("xml");

            foreach (var entry in scoreboard)
            {
                XElement xmlEntry = new XElement("entry",
                    new XElement("user", entry.name ?? "Voodooperson05"),
                    new XElement("score", entry.score),
                    new XElement("time", entry.time ?? "000"));

                xmlScoreboard.Add(xmlEntry);
            }

            return xmlScoreboard.ToString();
        }

        public static void UpdateScoreboardXml(string apiPath)
        {
            Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil");
            File.WriteAllText($"{apiPath}/juggernaut/clearasil/scoreboard.xml", ConvertScoreboardToXml());
        }
    }
}
