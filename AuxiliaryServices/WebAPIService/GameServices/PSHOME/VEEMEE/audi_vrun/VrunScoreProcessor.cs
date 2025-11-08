using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_vrun
{
    internal static class VrunScoreProcessor
    {
        private static VrunScoreBoardData _leaderboard = null;

        public static void InitializeLeaderboard()
        {
            if (_leaderboard == null)
                _leaderboard = new VrunScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);
        }

        public static string SetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        float time = (float)double.Parse(data.GetParameterValue("time"), CultureInfo.InvariantCulture);
                        float dist = (float)double.Parse(data.GetParameterValue("dist"), CultureInfo.InvariantCulture);

                        InitializeLeaderboard();

                        int numOfRaces = _leaderboard.GetNumOfRacesForUser(psnid);

                        _ = _leaderboard.UpdateScoreAsync(psnid, dist, new List<object> { numOfRaces++, time });

                        return $"<scores><entry><psnid>{psnid}</psnid><races>{numOfRaces}</races><distance>{dist.ToString().Replace(",", ".")}</distance><time>{time.ToString().Replace(",", ".")}</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - SetUserDataPOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");

                        InitializeLeaderboard();

                        if (_leaderboard != null)
                            return $"<scores><entry><psnid>{psnid}</psnid><races>{_leaderboard.GetNumOfRacesForUser(psnid)}</races><distance>{_leaderboard.GetScoreForUser(psnid)}</distance><time>{_leaderboard.GetTimeForUser(psnid)}</time></entry></scores>";

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><distance>0</distance><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetUserDataPOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetHigherUserScorePOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");

                        InitializeLeaderboard();

                        if (_leaderboard != null)
                        {
                            var entries = _leaderboard.GetTopScoresAsync(1).Result;

                            if (entries.Any())
                            {
                                var entry = entries.First();
                                return $"<scores><entry><psnid>{psnid}</psnid><races>{entry.numOfRaces}</races><distance>{entry.Score}</distance><time>{entry.time}</time></entry></scores>";
                            }
                        }

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><distance>0</distance><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetGlobalTablePOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        string title = data.GetParameterValue("title");

                        InitializeLeaderboard();

                        return _leaderboard?.SerializeToString(title).Result ?? $"<XML><PAGE><RECT X=\"0\" Y=\"1\" W=\"0\" H=\"0\" col=\"#C0C0C0\" /><RECT X=\"0\" Y=\"0\" W=\"1280\" H=\"720\" col=\"#000000\" /><TEXT X=\"57\" Y=\"42\" col=\"#FFFFFF\" size=\"4\">{title}</TEXT></PAGE></XML>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }
    }
}
