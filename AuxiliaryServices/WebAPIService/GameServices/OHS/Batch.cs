using System;
using System.IO;
using CustomLogger;
using Newtonsoft.Json.Linq;
using HttpMultipartParser;
using Newtonsoft.Json;
using System.Text;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.OHS
{
    public class Batch
    {
        public static string Batch_Process(byte[] PostData, string ContentType, string directorypath, int game)
        {
            string dataforohs = null;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                    dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                    ms.Flush();
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON data into a list of commands.
                    var commands = JsonConvert.DeserializeObject<BatchCommand[]>(dataforohs);

                    if (commands != null)
                    {
                        int i = 0;

                        StringBuilder resultBuilder = new StringBuilder();

                        foreach (var command in commands)
                        {
                            i++;

                            if (command != null && command.Data != null)
                            {
                                string resultfromcommand = null;
                                string method = command.Method;
                                string project = command.Project;
                                string data = command.Data.ToString(Formatting.None);

                                if (project == "<dummy>")
                                    project = "dummy";

                                Directory.CreateDirectory(directorypath + $"/{project}/");

                                LoggerAccessor.LogInfo($"[OHS] : Client Requested a /batch/ method, here are the details : method | {method} - project | {project} - data | {data}");

                                switch (method)
                                {
                                    case "community/getscore/":
                                        resultfromcommand = Community.Community_Getscore(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "community/updatescore/":
                                        resultfromcommand = Community.Community_UpdateScore(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "global/getall/":
                                        resultfromcommand = User.Get_All(PostData, ContentType, directorypath + $"/{project}/", data, true, game);
                                        break;
                                    case "global/gets/":
                                        resultfromcommand = User.Gets(PostData, ContentType, directorypath + $"/{project}/", data, true, game);
                                        break;
                                    case "global/get/":
                                        resultfromcommand = User.Get(PostData, ContentType, directorypath + $"/{project}/", data, true, game);
                                        break;
                                    case "global/set/":
                                        resultfromcommand = User.Set(PostData, ContentType, directorypath + $"/{project}/", data, true, game);
                                        break;
                                    case "userid/":
                                        resultfromcommand = User.User_Id(PostData, ContentType, data, game);
                                        break;
                                    case "user/getall/":
                                        resultfromcommand = User.Get_All(PostData, ContentType, directorypath + $"/{project}/", data, false, game);
                                        break;
                                    case "user/get/":
                                        resultfromcommand = User.Get(PostData, ContentType, directorypath + $"/{project}/", data, false, game);
                                        break;
                                    case "user/gets/":
                                        resultfromcommand = User.Gets(PostData, ContentType, directorypath + $"/{project}/", data, false, game);
                                        break;
                                    case "user/getmany/":
                                        resultfromcommand = User.GetMany(PostData, ContentType, directorypath + $"/{project}/", data, false, game);
                                        break;
                                    case "user/set/":
                                    case "user/setifempty/":
                                        resultfromcommand = User.Set(PostData, ContentType, directorypath + $"/{project}/", data, false, game);
                                        break;
                                    case "user/clearentry/":
                                        resultfromcommand = User.ClearEntry(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "user/getwritekey/":
                                        resultfromcommand = User.User_GetWritekey(PostData, ContentType, data, game);
                                        break;
                                    case "leaderboard/requestbyusers/":
                                        resultfromcommand = Leaderboard.Leaderboard_RequestByUsers(PostData, ContentType, project, data, game);
                                        break;
                                    case "leaderboard/requestbyrank/":
                                        resultfromcommand = Leaderboard.Leaderboard_RequestByRank(PostData, ContentType, project, data, game);
                                        break;
                                    case "leaderboard/update/":
                                        resultfromcommand = Leaderboard.Leaderboard_Update(PostData, ContentType, project, data, game, false);
                                        break;
                                    case "leaderboard/updatessameentry/":
                                        resultfromcommand = Leaderboard.Leaderboard_UpdatesSameEntry(PostData, ContentType, project, data, game, false);
                                        break;
                                    case "levelboard/getall/":
                                        resultfromcommand = Leaderboard.Levelboard_GetAll(project, game, true);
                                        break;
                                    case "levelboard/update/":
                                        resultfromcommand = Leaderboard.Leaderboard_Update(PostData, ContentType, project, data, game, true);
                                        break;
                                    case "usercounter/set/":
                                        resultfromcommand = UserCounter.Set(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "usercounter/increment/v2/":
                                        resultfromcommand = UserCounter.Increment(PostData, ContentType, directorypath + $"/{project}/", data, game, true);
                                        break;
                                    case "usercounter/increment/":
                                        resultfromcommand = UserCounter.Increment(PostData, ContentType, directorypath + $"/{project}/", data, game, false);
                                        break;
                                    case "usercounter/getall/":
                                        resultfromcommand = UserCounter.Get_All(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "usercounter/get/":
                                        resultfromcommand = UserCounter.Get(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "usercounter/incrementmany/":
                                        resultfromcommand = UserCounter.Increment_Many(PostData, ContentType, directorypath, data, game);
                                        break;
                                    case "usercounter/increment_setentry/":
                                        resultfromcommand = UserCounter.IncrementSetEntry(PostData, ContentType, directorypath, data, game);
                                        break;
                                    case "userinventory/addglobalitems/":
                                        resultfromcommand = UserInventory.AddGlobalItems(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "userinventory/getglobalitems/":
                                        resultfromcommand = UserInventory.GetGlobalItems(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "userinventory/updateuserinventory/":
                                        resultfromcommand = UserInventory.UpdateUserInventory(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "userinventory/getuserinventory/":
                                        resultfromcommand = UserInventory.GetUserInventory(PostData, ContentType, directorypath + $"/{project}/", data, game);
                                        break;
                                    case "register/":
                                        resultfromcommand = "{ }"; // Just bounce for now.
                                        break;
                                    case "voucher/":
                                        resultfromcommand = "{ [\"voucher\"] = \"xxxx-xxxx-xxxx\", [\"fresh\"] = true, [\"exhausted\"] = true }";
                                        break;
                                    default:
                                        LoggerAccessor.LogWarn($"[OHS] - Batch requested a method I don't know about, please report it to GITHUB {method} in {project} with data {data}");
                                        break;
                                }

                                if (string.IsNullOrEmpty(resultfromcommand))
                                    resultfromcommand = "{ [\"status\"] = \"fail\" }";

                                if (resultBuilder.Length == 0)
                                    resultBuilder.Append($"{{ [\"status\"] = \"success\", [\"value\"] = {{ [{i}] = {resultfromcommand}");
                                else
                                    resultBuilder.Append($", [{i}] = {resultfromcommand}");
                            }
                        }

                        resultBuilder.Append(" } }");
                        dataforohs = resultBuilder.ToString();
                        resultBuilder = null;
                    }
                    else
                        dataforohs = "{ [\"status\"] = \"fail\" }";
                }
                else
                    dataforohs = "{ [\"status\"] = \"fail\" }";
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Batch] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(dataforohs))
                dataforohs = JaminProcessor.JaminFormat(dataforohs, game);

            return dataforohs;
        }

        public class BatchCommand
        {
            public string Method { get; set; }
            public JToken Data { get; set; }
            public string Project { get; set; }
        }
    }
}
