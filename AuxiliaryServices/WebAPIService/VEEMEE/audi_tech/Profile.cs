using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace WebAPIService.VEEMEE.audi_tech
{
    public static class Profile
    {
        private const string DefaultProfile = @"{
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
            string psnid = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                    psnid = data.GetParameterValue("psnid");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                string verificationSalt = Processor.GetVerificationSalt(hex, new Dictionary<string, string> { { "psnid", psnid } });

                if (!__salt.Equals(verificationSalt))
                {
                    CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.Profile] - GetProfile - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}");
                    return null;
                }

                if (!string.IsNullOrEmpty(psnid))
                {
                    string profilePath = $"{apiPath}/VEEMEE/Audi_Tech/{psnid}/Profile.json";

                    if (File.Exists(profilePath))
                        return Processor.Sign(File.ReadAllText(profilePath));

                    return Processor.Sign(DefaultProfile.Replace("PUT_MYPSNID_HERE", psnid));
                }
            }

            return null;
        }

        public static string SetProfile(byte[] PostData, string ContentType, string apiPath)
        {
            string profile = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                    profile = data.GetParameterValue("profile");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    string verificationSalt = Processor.GetVerificationSalt(hex, new Dictionary<string, string> { { "profile", profile } });

                    if (!__salt.Equals(verificationSalt))
                    {
                        CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.Profile] - SetProfile - Invalid hex sent! Calculated:{verificationSalt} - Expected:{__salt}");
                        return null;
                    }

                    if (!string.IsNullOrEmpty(profile))
                    {
                        string psnID = JObject.Parse(profile)["psnID"].ToString();

                        if (!string.IsNullOrEmpty(psnID))
                        {
                            try
                            {
                                Directory.CreateDirectory($"{apiPath}/VEEMEE/Audi_Tech/{psnID}");

                                File.WriteAllText($"{apiPath}/VEEMEE/Audi_Tech/{psnID}/Profile.json", profile);

                                foreach (FilePart multipartfile in data.Files)
                                {
                                    if (multipartfile.FileName.Equals("ghost.dat"))
                                    {
                                        try
                                        {
                                            string ghostDirectoryPath = $"{apiPath}/VEEMEE/Audi_Tech/{psnID}/" + JObject.Parse(profile)["chosenTrack"].ToString() + " "
                                                + JObject.Parse(profile)["chosenChallenge"].ToString() + " " + JObject.Parse(profile)["chosenModifiers"].ToString() + "/";

                                            Directory.CreateDirectory(ghostDirectoryPath);

                                            using (Stream filedata = multipartfile.Data)
                                            {
                                                filedata.Position = 0;

                                                // Find the number of bytes in the stream
                                                int contentLength = (int)filedata.Length;

                                                // Create a byte array
                                                byte[] buffer = new byte[contentLength];

                                                // Read the contents of the memory stream into the byte array
                                                filedata.Read(buffer, 0, contentLength);

                                                File.WriteAllBytes($"{ghostDirectoryPath}ghost.dat", buffer);
                                            }
                                        }
                                        catch (JsonReaderException)
                                        {
                                            CustomLogger.LoggerAccessor.LogError($"[VEEMEE.audi_tech.Profile] - SetProfile - Invalid json profile was sent! Ignoring Ghost upload.");
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
