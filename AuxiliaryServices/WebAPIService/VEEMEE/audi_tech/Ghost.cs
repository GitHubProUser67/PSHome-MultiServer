using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace WebAPIService.VEEMEE.audi_tech
{
    public static class Ghost
    {
        public static string getFriendsGhostTimes(byte[] PostData, string ContentType, string apiPath)
        {
            string friends = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                    friends = data.GetParameterValue("friends");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                string verificationSalt = Processor.GetVerificationSalt(hex, new Dictionary<string, string> { { "friends", friends } });

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.GhostTimes] - getFriendsGhostTimes - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}");
                    return null;
                }

                if (!string.IsNullOrEmpty(friends))
                {
                    string[] friendsArray = JsonConvert.DeserializeObject<string[]>(friends);

                    if (friendsArray != null && friendsArray.Length > 0)
                    {
                        StringBuilder friendSerializer = new StringBuilder("{");

                        foreach (string psnid in friendsArray)
                        {
                            StringBuilder profileSerializer = new StringBuilder($"\"{psnid}\":[[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23],");

                            string profilePath = $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/Profile.json";

                            if (File.Exists(profilePath))
                            {
                                try
                                {
                                    using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(profilePath)))
                                    {
                                        StringBuilder scoreSerializer = new StringBuilder("[");

                                        for (byte i = 0; i < 24; i++)
                                        {
                                            double? score = GetTotalScore(document, $"{(i % 3) + 1} {Math.Floor((double)((i / 3) % 2)) + 1} {Math.Floor((double)((i / 6) + 1))}");

                                            if (score.HasValue && score.Value > 0)
                                            {
                                                if (scoreSerializer.Length == 1)
                                                    scoreSerializer.Append(score.Value.ToString().Replace(",", "."));
                                                else
                                                    scoreSerializer.Append("," + score.Value.ToString().Replace(",", "."));
                                            }
                                        }

                                        profileSerializer.Append(scoreSerializer.ToString() + "]");
                                    }
                                }
                                catch
                                {
                                    // Silence the error and send default value instead.
                                    profileSerializer.Append("[]");
                                }
                            }
                            else
                                profileSerializer.Append("[]");

                            profileSerializer.Append("]");

                            if (friendSerializer.Length == 1)
                                friendSerializer.Append(profileSerializer);
                            else
                                friendSerializer.Append($",{profileSerializer.ToString()}");
                        }

                        friendSerializer.Append("}");

                        return Processor.Sign(friendSerializer.ToString());
                    }
                }
            }

            return null;
        }

        public static byte[] getGhost(byte[] PostData, string ContentType, string apiPath)
        {
            string gameDef_3 = string.Empty;
            string gameDef_2 = string.Empty;
            string gameDef_1 = string.Empty;
            string psnid = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                    gameDef_3 = data.GetParameterValue("gameDef_3");
                    gameDef_2 = data.GetParameterValue("gameDef_2");
                    gameDef_1 = data.GetParameterValue("gameDef_1");
                    psnid = data.GetParameterValue("psnid");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                string verificationSalt = Processor.GetVerificationSalt(hex, new Dictionary<string, string> { { "gameDef_3", gameDef_3 }, { "gameDef_2", gameDef_2 },
                    { "gameDef_1", gameDef_1 }, { "psnid", psnid } });

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.GhostTimes] - getGhost - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}");
                    return null;
                }

                if (!string.IsNullOrEmpty(psnid))
                {
                    string ghostDirectoryPath = $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/" + gameDef_1 + " "
                    + gameDef_2 + " " + gameDef_3 + "/";

                    string ghostPath = $"{ghostDirectoryPath}ghost.dat";

                    if (File.Exists(ghostPath))
                        return File.ReadAllBytes(ghostPath);
                }
            }

            return null;
        }

        private static double? GetTotalScore(JsonDocument document, string identifier)
        {
            try
            {
                if (document.RootElement.TryGetProperty("hiScores", out JsonElement hiScores))
                {
                    if (hiScores.TryGetProperty(identifier, out JsonElement scoreElement))
                    {
                        if (scoreElement.TryGetProperty("totalScore", out JsonElement totalScore))
                            return totalScore.GetDouble();
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.GhostTimes] - GetTotalScore - An exception was thrown while parsing the json profile: {ex}");
            }

            return null;
        }
    }
}
