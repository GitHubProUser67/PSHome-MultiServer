using System.IO;
using System.Collections.Generic;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.PREMIUMAGENCY
{
    public class InfoBoard
    {
        private static HashSet<string> validLoungesCache = null;
        private static string loungesJsonPathCache = null;

        private static readonly HashSet<string> DefaultLounges = new HashSet<string>
        {
            "HomeSquare",
            "Cafe",
            "Theater",
            "GameSpace",
            "MarketPlace"
        };

        private static HashSet<string> LoadValidLounges(string jsonPath)
        {
            if (validLoungesCache != null && loungesJsonPathCache == jsonPath)
                return validLoungesCache;

            if (!File.Exists(jsonPath))
                return DefaultLounges;

            try
            {
                var json = File.ReadAllText(jsonPath);
                var root = JObject.Parse(json);
                var loungesSet = new HashSet<string>();
                foreach (var lounge in root["lounges"])
                {
                    loungesSet.Add(lounge.ToString());
                }
                validLoungesCache = loungesSet;
                loungesJsonPathCache = jsonPath;
                return loungesSet.Count > 0 ? loungesSet : DefaultLounges;
            }
            catch
            {
                return DefaultLounges;
            }
        }

        public static string getInformationBoardSchedulePOST(byte[] PostData, string ContentType, string workpath, string eventId)
        {
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);
            string lounge = string.Empty;
            string lang = string.Empty;
            string regcd = string.Empty;

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                lounge = data.GetParameterValue("lounge");
                lang = data.GetParameterValue("lang");
                regcd = data.GetParameterValue("regcd");
            }

            // Use the original string interpolation style
            string infoBoardSchedulePath = $"{workpath}/eventController/InfoBoards/Schedule";
			
            Directory.CreateDirectory(infoBoardSchedulePath);

            string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";

            if (LoadValidLounges($"{infoBoardSchedulePath}/lounges.json").Contains(lounge))
            {
                if (File.Exists(filePath))
                {
                    LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found and sent!");
                    return File.ReadAllText(filePath);
                }
                else
                    LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}. Expected path {filePath}!");
            }
            else
                LoggerAccessor.LogError($"[PREMIUMAGENCY] - Unsupported scene lounge {lounge} found for InfoBoardSchedule");

            return null;
        }
    }
}

