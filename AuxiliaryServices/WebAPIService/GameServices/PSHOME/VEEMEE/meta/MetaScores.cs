using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.meta
{
    public class MetaScores
    {
        private const string ValidKey = "JPDFC10A9MXS8HHOMOUKYAR3";

        public static string SetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (string.IsNullOrEmpty(boundary) || PostData == null)
                return null;

            try
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    string key = data.GetParameterValue("key");
                    if (key != ValidKey)
                    {
                        CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - SetUserData: invalid key.");
                        return null;
                    }

                    string psnid = data.GetParameterValue("psnid");
                    string game_id = data.GetParameterValue("game_id");
                    string sort_1 = data.GetParameterValue("sort_1");
                    string sort_2 = data.GetParameterValue("sort_2");
                    string score_1 = data.GetParameterValue("score_1");
                    string score_2 = data.GetParameterValue("score_2");

                    string directoryPath = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_1}/User_Data";
                    string directoryPath_2 = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_2}/User_Data";
                    string filePath = $"{directoryPath}/{psnid}.xml";
                    string filePath_2 = $"{directoryPath_2}/{psnid}_2.xml";

                    Directory.CreateDirectory(directoryPath);
                    Directory.CreateDirectory(directoryPath_2);

                    File.WriteAllText(filePath, $"<score>{score_1}</score>");
                    File.WriteAllText(filePath_2, $"<score>{score_2}</score>");

                    return $"<score><player><psn>{psnid}</psn><score_1>{score_1}</score_1><score_2>{score_2}</score_2></player></score>";
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[VEEMEE] - meta_scores - SetUserDataPOST exception: {ex}");
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (string.IsNullOrEmpty(boundary) || PostData == null)
                return null;

            try
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    string key = data.GetParameterValue("key");
                    if (key != ValidKey)
                    {
                        CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - GetUserData: invalid key.");
                        return null;
                    }

                    string psnid = data.GetParameterValue("psnid");
                    string game_id = data.GetParameterValue("game_id");
                    string sort_1 = data.GetParameterValue("sort_1");
                    string sort_2 = data.GetParameterValue("sort_2");

                    string filePath = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_1}/User_Data/{psnid}.xml";
                    string filePath_2 = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_2}/User_Data/{psnid}_2.xml";

                    if (File.Exists(filePath) && File.Exists(filePath_2))
                    {
                        string score_1 = File.ReadAllText(filePath).Replace("<score>", string.Empty).Replace("</score>", string.Empty).Trim();
                        string score_2 = File.ReadAllText(filePath_2).Replace("<score>", string.Empty).Replace("</score>", string.Empty).Trim();
                        return $"<score><player><psn>{psnid}</psn><score_1>{score_1}</score_1><score_2>{score_2}</score_2></player></score>";
                    }

                    return $"<score><player><psn>{psnid}</psn><score_1>0</score_1><score_2>0</score_2></player></score>";
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[VEEMEE] - meta_scores - GetUserDataPOST exception: {ex}");
            }

            return null;
        }
        public static string GetHighScoresPOST(byte[] PostData, string boundary, string apiPath, int filter = 0, bool friends = false)
        {
            if (string.IsNullOrEmpty(boundary) || PostData == null)
                return "<leaderboard></leaderboard>";

            try
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    string key = data.GetParameterValue("key");
                    if (key != ValidKey)
                    {
                        CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - GetHighScores: invalid key.");
                        return "<leaderboard></leaderboard>";
                    }

                    string game_id = data.GetParameterValue("game_id");
                    string sort_1 = data.GetParameterValue("sort_1");

                    if (string.IsNullOrEmpty(game_id))
                    {
                        CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - GetHighScores: missing game_id.");
                        return "<leaderboard></leaderboard>";
                    }

                    string userDataPath = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_1}/User_Data";

                    if (!Directory.Exists(userDataPath))
                        return "<leaderboard></leaderboard>";

                    HashSet<string> friendsFilter = null;
                    if (friends)
                    {
                        friendsFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var param in data.Parameters.Where(p => p.Name == "friends[]"))
                            friendsFilter.Add(param.Data);
                    }

                    DateTime? dateFrom = null;
                    DateTime? dateTo = null;
                    if (filter == 1) // Today
                    {
                        dateFrom = DateTime.UtcNow.Date;
                        dateTo = dateFrom.Value.AddDays(1);
                    }
                    else if (filter == 2) 
                    {
                        dateFrom = DateTime.UtcNow.Date.AddDays(-1);
                        dateTo = DateTime.UtcNow.Date;
                    }

                    var entries = new List<(string psnid, long score1, long score2)>();

                    foreach (string filePath in Directory.EnumerateFiles(userDataPath, "*.xml"))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        if (fileName.EndsWith("_2"))
                            continue;

                        if (dateFrom.HasValue)
                        {
                            DateTime lastWrite = File.GetLastWriteTimeUtc(filePath);
                            if (lastWrite < dateFrom.Value || lastWrite >= dateTo.Value)
                                continue;
                        }

                        if (friendsFilter != null && !friendsFilter.Contains(fileName))
                            continue;

                        long score1 = 0;
                        try
                        {
                            string raw = File.ReadAllText(filePath)
                                .Replace("<score>", string.Empty)
                                .Replace("</score>", string.Empty)
                                .Trim();
                            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double d1))
                                score1 = (long)d1;
                        }
                        catch { continue; }

                        long score2 = 0;
                        string filePath_2 = Path.Combine(userDataPath, fileName + "_2.xml");
                        if (File.Exists(filePath_2))
                        {
                            try
                            {
                                string raw2 = File.ReadAllText(filePath_2)
                                    .Replace("<score>", string.Empty)
                                    .Replace("</score>", string.Empty)
                                    .Trim();
                                if (double.TryParse(raw2, NumberStyles.Float, CultureInfo.InvariantCulture, out double d2))
                                    score2 = (long)d2;
                            }
                            catch { }
                        }

                        entries.Add((fileName, score1, score2));
                    }

                    if (sort_1.Equals("DESC", StringComparison.OrdinalIgnoreCase))
                        entries = entries.OrderByDescending(e => e.score1).ToList();
                    else
                        entries = entries.OrderBy(e => e.score1).ToList();

                    var sb = new StringBuilder();
                    sb.Append("<leaderboard>");
                    int limit = Math.Min(10, entries.Count);
                    for (int i = 0; i < limit; i++)
                    {
                        var e = entries[i];
                        sb.Append("<player>");
                        sb.Append($"<psn>{e.psnid}</psn>");
                        sb.Append("<psnid>0</psnid>");
                        sb.Append($"<score_1>{e.score1}</score_1>");
                        sb.Append($"<score_2>{e.score2}</score_2>");
                        sb.Append("</player>");
                    }
                    sb.Append("</leaderboard>");
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[VEEMEE] - meta_scores - GetHighScoresPOST exception: {ex}");
            }

            return "<leaderboard></leaderboard>";
        }
    }
}