using System.IO;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;

namespace WebAPIService.PREMIUMAGENCY
{
    public class InfoBoard
    {
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

                ms.Flush();
            }

            #region InfoBoard Paths

            string infoBoardSchedulePath = $"{workpath}/eventController/InfoBoards/Schedule";

            #endregion

            switch (lounge)
            {
                case "HomeSquare":
                    {
                        Directory.CreateDirectory(infoBoardSchedulePath);
                        string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found with {filePath} and sent!");
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}\nExpected path {filePath}!");
                        }
                        break;
                    }
                case "Cafe":
                    {
                        Directory.CreateDirectory(infoBoardSchedulePath);
                        string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found and sent!");
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}\nExpected path {filePath}!");
                        }
                        break;
                    }
                case "Theater":
                    {
                        Directory.CreateDirectory(infoBoardSchedulePath);
                        string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found and sent!");
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}\nExpected path {filePath}!");
                        }
                        break;
                    }
                case "GameSpace":
                    {
                        Directory.CreateDirectory(infoBoardSchedulePath);
                        string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found and sent!");
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}\nExpected path {filePath}!");
                        }
                        break;
                    }
                case "MarketPlace":
                    {
                        Directory.CreateDirectory(infoBoardSchedulePath);
                        string filePath = $"{infoBoardSchedulePath}/{lounge}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - InfoBoardSchedule for {lounge} found and sent!");
                            return File.ReadAllText(filePath);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find InfoBoardSchedule for {lounge}\nExpected path {filePath}!");
                        }
                        break;
                    }
                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - Unsupported scene lounge {lounge} found for InfoBoardSchedule");
                        return null;
                    }

                    
            }
            
            return null;
        }


    }
}
