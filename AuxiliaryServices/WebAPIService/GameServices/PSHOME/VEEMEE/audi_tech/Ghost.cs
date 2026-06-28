using System.Text;
using System.Text.Json;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_tech
{
    public static class Ghost
    {
        public static string getFriendsGhostTimes(
            byte[] PostData,
            string ContentType,
            string apiPath
        )
        {
            var friends = string.Empty;
            var hex = string.Empty;
            var __salt = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    friends = data.GetParameterValue("friends");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                var verificationSalt = Processor.GetVerificationSalt(
                    hex,
                    new Dictionary<string, string> { { "friends", friends } }
                );

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[VEEMEE.audi_tech.GhostTimes] - getFriendsGhostTimes - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}"
                    );
                    return null;
                }

                if (!string.IsNullOrEmpty(friends))
                {
                    var friendsArray = JsonConvert.DeserializeObject<string[]>(friends);

                    if (friendsArray != null && friendsArray.Length > 0)
                    {
                        var friendSerializer = new StringBuilder("{");

                        foreach (var psnid in friendsArray)
                        {
                            var profileSerializer = new StringBuilder(
                                $"\"{psnid}\":[[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23],"
                            );

                            var profilePath = $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/Profile.json";

                            if (File.Exists(profilePath))
                            {
                                try
                                {
                                    using (
                                        var document = JsonDocument.Parse(
                                            File.ReadAllText(profilePath)
                                        )
                                    )
                                    {
                                        var scoreSerializer = new StringBuilder("[");

                                        for (byte i = 0; i < 24; i++)
                                        {
                                            var score = GetTotalScore(
                                                document,
                                                $"{(i % 3) + 1} {Math.Floor((double)(i / 3 % 2)) + 1} {Math.Floor((double)((i / 6) + 1))}"
                                            );

                                            if (score.HasValue && score.Value > 0)
                                            {
                                                if (scoreSerializer.Length == 1)
                                                    scoreSerializer.Append(
                                                        score.Value.ToString().Replace(",", ".")
                                                    );
                                                else
                                                    scoreSerializer.Append(
                                                        ","
                                                            + score
                                                                .Value.ToString()
                                                                .Replace(",", ".")
                                                    );
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

                            profileSerializer.Append(']');

                            if (friendSerializer.Length == 1)
                                friendSerializer.Append(profileSerializer);
                            else
                                friendSerializer.Append($",{profileSerializer.ToString()}");
                        }

                        friendSerializer.Append('}');

                        return Processor.Sign(friendSerializer.ToString());
                    }
                }
            }

            return null;
        }

        public static byte[] getGhost(byte[] PostData, string ContentType, string apiPath)
        {
            var gameDef_3 = string.Empty;
            var gameDef_2 = string.Empty;
            var gameDef_1 = string.Empty;
            var psnid = string.Empty;
            var hex = string.Empty;
            var __salt = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    gameDef_3 = data.GetParameterValue("gameDef_3");
                    gameDef_2 = data.GetParameterValue("gameDef_2");
                    gameDef_1 = data.GetParameterValue("gameDef_1");
                    psnid = data.GetParameterValue("psnid");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                var verificationSalt = Processor.GetVerificationSalt(
                    hex,
                    new Dictionary<string, string>
                    {
                        { "gameDef_3", gameDef_3 },
                        { "gameDef_2", gameDef_2 },
                        { "gameDef_1", gameDef_1 },
                        { "psnid", psnid },
                    }
                );

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[VEEMEE.audi_tech.GhostTimes] - getGhost - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}"
                    );
                    return null;
                }

                if (!string.IsNullOrEmpty(psnid))
                {
                    var ghostDirectoryPath =
                        $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/"
                        + gameDef_1
                        + " "
                        + gameDef_2
                        + " "
                        + gameDef_3
                        + "/";

                    var ghostPath = $"{ghostDirectoryPath}ghost.dat";

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
                if (document.RootElement.TryGetProperty("hiScores", out var hiScores))
                {
                    if (hiScores.TryGetProperty(identifier, out var scoreElement))
                    {
                        if (scoreElement.TryGetProperty("totalScore", out var totalScore))
                            return totalScore.GetDouble();
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError(
                    $"[VEEMEE.audi_tech.GhostTimes] - GetTotalScore - An exception was thrown while parsing the json profile: {ex}"
                );
            }

            return null;
        }
    }
}
