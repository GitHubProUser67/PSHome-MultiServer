using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.HFProcessors
{
    public class ClearasilSkaterRequestProcessor
    {
        public static string ProcessMainPHP(
            byte[] PostData,
            string ContentType,
            string PHPSessionID,
            string WorkPath
        )
        {
            if (PostData == null || string.IsNullOrEmpty(ContentType))
                return null;

            var Command = string.Empty;
            var UserID = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (boundary != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    Command = data.GetParameterValue("Command");
                    UserID = data.GetParameterValue("UserID");

                    LoggerAccessor.LogInfo($"[HFGAMES] Command detected as {Command}");

                    try
                    {
                        var DisplayName = data.GetParameterValue("DisplayName");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        var InstanceID = data.GetParameterValue("InstanceID");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        var Region = data.GetParameterValue("Region");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    ms.Flush();
                }

                if (!string.IsNullOrEmpty(Command))
                {
                    Directory.CreateDirectory($"{WorkPath}/ClearasilSkater/User_Data");

                    switch (Command)
                    {
                        case "RequestNPTicket":
                            return NPTicket.RequestNPTicket(PostData, boundary);
                        case "RequestUser":
                            return User.RequestUserClearasilSkater(
                                PostData,
                                boundary,
                                UserID,
                                WorkPath
                            );
                        case "UpdateUser":
                            return User.UpdateUserClearasilSkater(
                                PostData,
                                boundary,
                                UserID,
                                WorkPath
                            );
                        case "TotalScoreLeaderboard":
                            return Leaderboards.GetLeaderboardsClearasil(
                                PostData,
                                boundary,
                                UserID,
                                WorkPath
                            );
                        case "LogMetric":
                            return "<Response></Response>"; // We don't really care about Metrics just yet

                        case "QueryMotd":
                            return "<Response><Motd>Message of the Day!</Motd></Response>";
                        case "QueryServerGlobals":
                            return "<Response><GlobalHard>1</GlobalHard><GlobalWrinkles>1</GlobalWrinkles></Response>";
                        case "QueryHoldbacks":
                            return "<Response></Response>";
                        case "QueryRewards":
                            return File.Exists(
                                $"{WorkPath}/ClearasilSkater/User_Data/{UserID}_Rewards.xml"
                            )
                                ? $"<Response>{File.ReadAllText($"{WorkPath}/TYCOON/User_Data/{UserID}_Rewards.xml")}</Response>"
                                : "<Response></Response>";
                        case "QueryGifts":
                            return File.Exists(
                                $"{WorkPath}/ClearasilSkater/User_Data/{UserID}_Gifts.xml"
                            )
                                ? $"<Response>{File.ReadAllText($"{WorkPath}/TYCOON/User_Data/{UserID}_Gifts.xml")}</Response>"
                                : "<Response><Gift>111111</Gift></Response>";
                        default:
                            LoggerAccessor.LogWarn(
                                $"[HFGAMES] - Client Request a Command I don't know about, please post the message on GITHUB : {Command}"
                            );
                            return "<Response></Response>";
                    }
                }
            }

            return null;
        }
    }
}
