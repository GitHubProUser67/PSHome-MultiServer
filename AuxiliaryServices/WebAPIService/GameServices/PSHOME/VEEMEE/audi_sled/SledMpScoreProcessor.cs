using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_sled
{
    internal static class SledMpScoreProcessor
    {
        private static SledMpScoreBoardData _leaderboard = null;

        public static void InitializeLeaderboard()
        {
            if (_leaderboard == null)
                _leaderboard = new SledMpScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);
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
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_sledmp - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        float time = (float)double.Parse(data.GetParameterValue("time"), CultureInfo.InvariantCulture);
                        float points = (float)double.Parse(data.GetParameterValue("points"), CultureInfo.InvariantCulture);

                        InitializeLeaderboard();

                        int numOfRaces = _leaderboard.GetNumOfRacesForUser(psnid);

                        _ = _leaderboard.UpdateScoreAsync(psnid, points, new List<object> { numOfRaces++, time });

                        return $"<scores><entry><psnid>{psnid}</psnid><races>{numOfRaces}</races><points>{points.ToString().Replace(",", ".")}</points><time>{time.ToString().Replace(",", ".")}</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[SledMpScoreProcessor] - SetUserDataPOST thrown an assertion. (Exception: {ex})");
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
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_sledmp - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");

                        InitializeLeaderboard();

                        if (_leaderboard != null)
                            return $"<scores><entry><psnid>{psnid}</psnid><races>{_leaderboard.GetNumOfRacesForUser(psnid)}</races><points>{_leaderboard.GetScoreForUser(psnid)}</points><time>{_leaderboard.GetTimeForUser(psnid)}</time></entry></scores>";

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><points>0</points><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[SledMpScoreProcessor] - GetUserDataPOST thrown an assertion. (Exception: {ex})");
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
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_sledmp - Client tried to push invalid key! Invalidating request.");
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
                                return $"<scores><entry><psnid>{psnid}</psnid><races>{entry.numOfRaces}</races><points>{entry.Score}</points><time>{entry.time}</time></entry></scores>";
                            }
                        }

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><points>0</points><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[SledMpScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
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
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_sledmp - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        string title = data.GetParameterValue("title");

                        InitializeLeaderboard();

                        return _leaderboard?.SerializeToString(title).Result ?? $"<XML><PAGE><TEXT X=\"100\" Y=\"70\" col=\"#FFFFFF\" size=\"4\">{title}</TEXT></PAGE></XML>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[SledMpScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }
    }
}
