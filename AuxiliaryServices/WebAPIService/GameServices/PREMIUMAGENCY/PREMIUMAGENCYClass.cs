using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;





#if !NETFRAMEWORK
using System.Web;
#endif
namespace WebAPIService.GameServices.PREMIUMAGENCY
{
    public class PREMIUMAGENCYClass
    {
        private string workpath;
        private string absolutepath;
        private string fulluripath;
        private string method;

        public PREMIUMAGENCYClass(string method, string absolutepath, string workpath, string fulluripath)
        {
            this.workpath = workpath;
            this.absolutepath = absolutepath;
            this.fulluripath = fulluripath;
            this.method = method;
        }

        public string ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            string evid = string.Empty;

            if (ContentType == null)
            {
#if NETFRAMEWORK
                evid = HTTPProcessor.GetQueryParameters(fulluripath)["evid"];
#else
                evid = HttpUtility.ParseQueryString(fulluripath).Get("evid");
#endif
            }
            else
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    evid = data.GetParameterValue("evid");

                    ms.Flush();
                }
            }

            switch (method)
            {
                case "GET":
                    switch (absolutepath)
                    {
                        case "/eventController/getResource.do":
                            return Resource.getResourcePOST(PostData, ContentType, workpath, fulluripath, method);
                        case "/eventController/confirmEvent.do":
                        //return Event.confirmEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath); //Unimplemented 
                        case "/eventController/checkEvent.do":
                            return Event.checkEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/entryEvent.do":
                            return Event.entryEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/clearEvent.do":
                            return Event.clearEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/setUserEventCustom.do":
                            return Custom.setUserEventCustomPOST(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getUserEventCustom.do":
                            return Custom.getUserEventCustomRequestPOST(PostData, ContentType, workpath, evid, method);
                        case "/eventController/getUserEventCustomList.do":
                            return Custom.getUserEventCustomRequestListPOST(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getItemRankingTable.do":
                            return Ranking.getItemRankingTableHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/entryItemRankingPoints.do":
                            return Ranking.entryItemRankingPointsHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getItemRankingTargetList.do":
                            return Ranking.getItemRankingTargetListHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        default:
                            {
                                LoggerAccessor.LogError($"[PREMIUMAGENCY] - Unhandled {method} server request discovered: {absolutepath.Replace("/eventController/", "")} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}");
                            }
                            break;
                    }
                    break;

                case "POST":
                    switch (absolutepath)
                    {
                        case "/eventController/getResource.do":
                            return Resource.getResourcePOST(PostData, ContentType, workpath, fulluripath, method);
                        case "/eventController/confirmEvent.do":
                            return Event.confirmEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/checkEvent.do":
                            return Event.checkEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/entryEvent.do":
                            return Event.entryEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/clearEvent.do":
                            return Event.clearEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath, method);
                        case "/eventController/getEventTrigger.do":
                            return Trigger.getEventTriggerRequestPOST(PostData, ContentType, workpath, evid);
                        case "/eventController/getEventTriggerEx.do":
                            return Trigger.getEventTriggerExRequestPOST(PostData, ContentType, workpath, evid);
                        case "/eventController/confirmEventTrigger.do":
                            return Trigger.confirmEventTriggerRequestPOST(PostData, ContentType, workpath, evid);
                        case "/eventController/setUserEventCustom.do":
                            return Custom.setUserEventCustomPOST(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getUserEventCustom.do":
                            return Custom.getUserEventCustomRequestPOST(PostData, ContentType, workpath, evid, method);
                        case "/eventController/getUserEventCustomList.do":
                            return Custom.getUserEventCustomRequestListPOST(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getItemRankingTable.do":
                            return Ranking.getItemRankingTableHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/entryItemRankingPoints.do":
                            return Ranking.entryItemRankingPointsHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getItemRankingTargetList.do":
                            return Ranking.getItemRankingTargetListHandler(PostData, ContentType, workpath, evid, fulluripath, method);
                        case "/eventController/getInformationBoardSchedule.do":
                            return InfoBoard.getInformationBoardSchedulePOST(PostData, ContentType, workpath, evid);
                        case "/eventController/checkAccount.do":
                            return Account.checkAccount(PostData, ContentType, workpath);
                        case "/eventController/entryAccount.do":
                            return Account.entryAccount(PostData, ContentType, workpath);
                        case "/eventController/confirmAccount.do":
                            //return Account.confirmAccount(PostData, ContentType, workpath, evid);
                        default:
                            {
                                LoggerAccessor.LogError($"[PREMIUMAGENCY] - Unhandled {method} server request discovered: {absolutepath.Replace("/eventController/", "")} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}");
                            }
                        break;
                    }
                break;

                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - Unhandled Server method: {method}");
                    }
                    break;
            }

            return null;
        }

        public static void WriteFormDataToFile(string formData, string filePath)
        {
            try
            {
                // Regular expression to match each key-value pair
                Regex regex = new Regex(@"name=""([^""]+)""\s*([\s\S]*?)\s*---------");
                MatchCollection matches = regex.Matches(formData);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Match match in matches)
                    {
                        string key = match.Groups[1].Value.Trim();
                        string value = match.Groups[2].Value.Trim();

                        // Write key-value pair to the file
                        writer.WriteLine($"{key}: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"Fatal exception occured in WriteFormDataToFile with exception:\n", ex);
            }
        }

        public static List<(string, string)> ReadFormDataFromFile(string filePath)
        {
            try
            {

                List<(string, string)> formData = new List<(string, string)>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = null;
                    string currentKey = string.Empty;
                    string currentValue = string.Empty;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(":"))
                        {
                            string[] parts = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                if (currentKey != null)
                                {
                                    formData.Add((currentKey.Trim(), currentValue.Trim()));
                                }
                                currentKey = parts[0].Trim();
                                currentValue = parts[1].Trim();
                            }
                        }
                        else
                            currentValue += "\n" + line.Trim();
                    }

                    // Add the last key-value pair
                    if (currentKey != null)
                        formData.Add((currentKey.Trim(), currentValue.Trim()));
                }

                return formData;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"Fatal exception occured in ReadFormDataFromFile with exception:\n", ex);
                return null;
            }
        }
    }
}