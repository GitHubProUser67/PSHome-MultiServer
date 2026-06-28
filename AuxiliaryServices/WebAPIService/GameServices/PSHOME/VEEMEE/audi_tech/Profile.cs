using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_tech
{
    public static class Profile
    {
        private const string DefaultProfile =
            @"{
                ""psnID"": ""PUT_MYPSNID_HERE"",
                ""chosenTrack"": 1,
                ""chosenChallenge"": 1,
                ""chosenModifiers"": 1,
                ""equippedQuattro"": 1,
                ""equippedTransmission"": 1,
                ""equippedUltra"": 1,
                ""chosenGhostType"": 1,
                ""raceStartCount"": 0,
                ""raceCompleteCount"": 0,
                ""raceQuitCount"": 0,
                ""timeSpentRacing"": 0,
                ""timeSpentInGarage"": 0,
                ""sessionCount"": 0,
                ""introPlayed"": false,
                ""isValid"": true,
                ""isDirty"": true,
                ""newUnlocks"": {},
		        ""unlockedQuattro"": [1],
		        ""unlockedTransmission"": [1],
		        ""unlockedUltra"": [1],
		        ""unlockedTracks"": [1],
		        ""unlockedChallenges"": [1],
		        ""unlockedModifiers"": [1],
		        ""medalsWon"": {""2 2 2"": 0, ""1 2 2"": 0, ""3 2 4"": 0, ""2 2 4"": 0, ""3 1 1"": 0, ""2 1 1"": 0, ""1 2 1"": 0, ""2 2 1"": 0, ""3 2 2"": 0, ""2 1 3"": 0, ""1 1 3"": 0, ""3 1 3"": 0, ""1 2 4"": 0, ""1 2 3"": 0, ""1 1 1"": 0, ""1 1 4"": 0, ""1 1 2"": 0, ""2 1 2"": 0, ""3 2 3"": 0, ""3 1 4"": 0, ""3 2 1"": 0, ""2 2 3"": 0, ""2 1 4"": 0, ""3 1 2"": 0},
		        ""ghostDefs"": {""2 2 2"": false, ""1 2 2"": false, ""3 2 4"": false, ""2 2 4"": false, ""3 1 1"": false, ""2 1 1"": false, ""1 2 1"": false, ""2 2 1"": false, ""3 2 2"": false, ""2 1 3"": false, ""1 1 3"": false, ""3 1 3"": false, ""1 2 4"": false, ""1 2 3"": false, ""1 1 1"": false, ""1 1 4"": false, ""1 1 2"": false, ""2 1 2"": false, ""3 2 3"": false, ""3 1 4"": false, ""3 2 1"": false, ""2 2 3"": false, ""2 1 4"": false, ""3 1 2"": false},
		        ""hiScores"": {""2 2 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 2 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 2 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 2 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 1 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 1 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 2 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 2 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 2 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 1 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 1 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 1 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 2 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 2 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 1 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 1 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""1 1 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 1 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 2 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 1 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 2 1"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 2 3"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""2 1 4"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}, ""3 1 2"": {""timeTaken"": 0, ""totalScore"": 0, ""efficiency"": 0, ""comfort"": 0, ""penalties"": 0}}
            }";

        public static string GetProfile(byte[] PostData, string ContentType, string apiPath)
        {
            var psnid = string.Empty;
            var hex = string.Empty;
            var __salt = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    psnid = data.GetParameterValue("psnid");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                var verificationSalt = Processor.GetVerificationSalt(
                    hex,
                    new Dictionary<string, string> { { "psnid", psnid } }
                );

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[VEEMEE.audi_tech.Profile] - GetProfile - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}"
                    );
                    return null;
                }

                if (!string.IsNullOrEmpty(psnid))
                {
                    var profilePath = $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/Profile.json";

                    return File.Exists(profilePath)
                        ? Processor.Sign(File.ReadAllText(profilePath))
                        : Processor.Sign(DefaultProfile.Replace("PUT_MYPSNID_HERE", psnid));
                }
            }

            return null;
        }

        public static string SetProfile(byte[] PostData, string ContentType, string apiPath)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var profile = data.GetParameterValue("profile");
                    var hex = data.GetParameterValue("hex");
                    var __salt = data.GetParameterValue("__salt");

                    var verificationSalt = Processor.GetVerificationSalt(
                        hex,
                        new Dictionary<string, string> { { "profile", profile } }
                    );

                    if (!__salt.Equals(verificationSalt))
                    {
                        CustomLogger.LoggerAccessor.LogError(
                            $"[VEEMEE.audi_tech.Profile] - SetProfile - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}"
                        );
                        return null;
                    }

                    if (!string.IsNullOrEmpty(profile))
                    {
                        var psnID = JObject.Parse(profile)["psnID"].ToString();

                        if (!string.IsNullOrEmpty(psnID))
                        {
                            try
                            {
                                Directory.CreateDirectory($"{apiPath}/VEEMEE/Audi_Tech/{psnID}");

                                File.WriteAllText(
                                    $"{apiPath}/VEEMEE/Audi_Tech/{psnID}/Profile.json",
                                    profile
                                );

                                foreach (var multipartfile in data.Files)
                                {
                                    if (multipartfile.FileName.Equals("ghost.dat"))
                                    {
                                        try
                                        {
                                            var ghostDirectoryPath =
                                                $"{apiPath}/VEEMEE/Audi_Tech/{psnID}/"
                                                + JObject.Parse(profile)["chosenTrack"].ToString()
                                                + " "
                                                + JObject
                                                    .Parse(profile)["chosenChallenge"]
                                                    .ToString()
                                                + " "
                                                + JObject
                                                    .Parse(profile)["chosenModifiers"]
                                                    .ToString()
                                                + "/";

                                            Directory.CreateDirectory(ghostDirectoryPath);

                                            using (var filedata = multipartfile.Data)
                                            {
                                                filedata.Position = 0;

                                                // Find the number of bytes in the stream
                                                var contentLength = (int)filedata.Length;

                                                // Create a byte array
                                                var buffer = new byte[contentLength];

                                                // Read the contents of the memory stream into the byte array
                                                filedata.Read(buffer, 0, contentLength);

                                                File.WriteAllBytes(
                                                    $"{ghostDirectoryPath}ghost.dat",
                                                    buffer
                                                );
                                            }
                                        }
                                        catch (JsonReaderException)
                                        {
                                            CustomLogger.LoggerAccessor.LogError(
                                                $"[VEEMEE.audi_tech.Profile] - SetProfile - Invalid json profile was sent! Ignoring Ghost upload."
                                            );
                                        }
                                    }
                                }

                                return "ok";
                            }
                            catch
                            {
                                // Ignore errors and simply return null.
                            }
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
        }
    }
}
