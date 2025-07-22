using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WebAPIService.GameServices.NDREAMS.Aurora
{
    public class OrbrunnerScoreBoardData
    {
        private static bool _initiated = false;

        private static object _Lock = new object();

        public class ScoreboardEntry
        {
            public string psnid { get; set; }
            public int score { get; set; }
        }

        private static List<ScoreboardEntry> scoreboard = new List<ScoreboardEntry>();

        private static void LoadScoreBoardFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _initiated = true;
                return;
            }

            // Split the input text by commas into pairs
            string[] parts = text.Split(',');

            if (parts.Length % 2 != 0)
            {
                CustomLogger.LoggerAccessor.LogError("[ndreams] - Orbrunner - Invalid scoreboard input format.");
                _initiated = true;
                return;
            }

            // Iterate over the pairs (psnid, score)
            for (int i = 0; i < parts.Length; i += 2)
            {
                string psnid = parts[i];
                if (!int.TryParse(parts[i + 1], out int score)) // Ensure valid score format
                {
                    CustomLogger.LoggerAccessor.LogWarn("[ndreams] - Orbrunner - Invalid score for player {psnid}. Skipping this entry.");
                    continue;
                }

                // Update or add the score entry to the scoreboard
                UpdateScoreBoard(psnid, score);
            }

            _initiated = true;
        }

        public static void UpdateScoreBoard(string psnid, int newScore)
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
                    scoreboard.Add(new ScoreboardEntry { psnid = psnid, score = newScore });
            }

            // Sort the scoreboard by score in descending order
            scoreboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Trim the scoreboard to the top 10 entries
            if (scoreboard.Count > 10)
                scoreboard.RemoveRange(10, scoreboard.Count - 10);
        }

        public static void CheckForInitiatedleaderboard(string apiPath)
        {
            if (!_initiated)
            {
                lock (_Lock)
                {
                    string filePath = $"{apiPath}/NDREAMS/Aurora/Orbrunner/leaderboard.txt";
                    Directory.CreateDirectory($"{apiPath}/NDREAMS/Aurora/Orbrunner");
                    if (File.Exists(filePath))
                        LoadScoreBoardFromText(File.ReadAllText(filePath));
                    else
                        _initiated = true;
                    CustomLogger.LoggerAccessor.LogDebug($"[ndreams] - Orbrunner - scoreboard initiated.");
                }
            }
        }

        public static (string, int)? GetHighestScore()
        {
            if (scoreboard.Count == 0)
                return null;

            ScoreboardEntry highestEntry = scoreboard.OrderByDescending(x => x.score > 0)
                                        .ThenBy(x => x.score)
                                        .ToList()[0];

            return (highestEntry.psnid, highestEntry.score);
        }

        public static string ConvertScoreboardToText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in scoreboard)
            {
                if (sb.Length == 0)
                    sb.Append(entry.psnid + "," + entry.score);
                else
                    sb.Append("," + entry.psnid + "," + entry.score);
            }

            return sb.ToString();
        }

        public static void UpdateScoreboardXml(string apiPath)
        {
            lock (_Lock)
            {
                string filePath = $"{apiPath}/NDREAMS/Aurora/Orbrunner/leaderboard.txt";
                Directory.CreateDirectory($"{apiPath}/NDREAMS/Aurora/Orbrunner");
                File.WriteAllText(filePath, ConvertScoreboardToText());
                CustomLogger.LoggerAccessor.LogDebug($"[ndreams] - Orbrunner - scoreboard updated.");
            }
        }
    }
}
