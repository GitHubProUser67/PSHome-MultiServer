using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.PREMIUMAGENCY
{
    public partial class PREMIUMAGENCYClass(
        string method,
        string absolutepath,
        string workpath,
        string fulluripath
    )
    {
        private readonly string workpath = workpath;
        private readonly string absolutepath = absolutepath;
        private readonly string fulluripath = fulluripath;
        private readonly string method = method;

        public string ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            var evid = string.Empty;

            if (ContentType == null)
            {
                evid = HttpUtility.ParseQueryString(fulluripath).Get("evid");
            }
            else
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (var ms = new MemoryStream(PostData))
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
                            return Resource.getResourcePOST(
                                PostData,
                                ContentType,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/confirmEvent.do":
                        //return Event.confirmEventRequestPOST(PostData, ContentType, evid, workpath, fulluripath); //Unimplemented
                        case "/eventController/checkEvent.do":
                            return Event.checkEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/entryEvent.do":
                            return Event.entryEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/clearEvent.do":
                            return Event.clearEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/setUserEventCustom.do":
                            return Custom.setUserEventCustomPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getUserEventCustom.do":
                            return Custom.getUserEventCustomRequestPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                method
                            );
                        case "/eventController/getUserEventCustomList.do":
                            return Custom.getUserEventCustomRequestListPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getItemRankingTable.do":
                            return Ranking.getItemRankingTableHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/entryItemRankingPoints.do":
                            return Ranking.entryItemRankingPointsHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getItemRankingTargetList.do":
                            return Ranking.getItemRankingTargetListHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        default:
                            {
                                LoggerAccessor.LogError(
                                    $"[PREMIUMAGENCY] - Unhandled {method} server request discovered: {absolutepath.Replace("/eventController/", "")} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}"
                                );
                            }
                            break;
                    }
                    break;

                case "POST":
                    switch (absolutepath)
                    {
                        case "/eventController/getResource.do":
                            return Resource.getResourcePOST(
                                PostData,
                                ContentType,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/confirmEvent.do":
                            return Event.confirmEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/checkEvent.do":
                            return Event.checkEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/entryEvent.do":
                            return Event.entryEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/clearEvent.do":
                            return Event.clearEventRequestPOST(
                                PostData,
                                ContentType,
                                evid,
                                workpath,
                                fulluripath,
                                method
                            );
                        case "/eventController/getEventTrigger.do":
                            return Trigger.getEventTriggerRequestPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid
                            );
                        case "/eventController/getEventTriggerEx.do":
                            return Trigger.getEventTriggerExRequestPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid
                            );
                        case "/eventController/confirmEventTrigger.do":
                            return Trigger.confirmEventTriggerRequestPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid
                            );
                        case "/eventController/setUserEventCustom.do":
                            return Custom.setUserEventCustomPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getUserEventCustom.do":
                            return Custom.getUserEventCustomRequestPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                method
                            );
                        case "/eventController/getUserEventCustomList.do":
                            return Custom.getUserEventCustomRequestListPOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getItemRankingTable.do":
                            return Ranking.getItemRankingTableHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/entryItemRankingPoints.do":
                            return Ranking.entryItemRankingPointsHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getItemRankingTargetList.do":
                            return Ranking.getItemRankingTargetListHandler(
                                PostData,
                                ContentType,
                                workpath,
                                evid,
                                fulluripath,
                                method
                            );
                        case "/eventController/getInformationBoardSchedule.do":
                            return InfoBoard.getInformationBoardSchedulePOST(
                                PostData,
                                ContentType,
                                workpath,
                                evid
                            );
                        case "/eventController/checkAccount.do":
                            return Account.checkAccount(PostData, ContentType, workpath);
                        case "/eventController/entryAccount.do":
                            return Account.entryAccount(PostData, ContentType, workpath);
                        case "/eventController/confirmAccount.do":
                        //return Account.confirmAccount(PostData, ContentType, workpath, evid);
                        default:
                            {
                                LoggerAccessor.LogError(
                                    $"[PREMIUMAGENCY] - Unhandled {method} server request discovered: {absolutepath.Replace("/eventController/", "")} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}"
                                );
                            }
                            break;
                    }
                    break;

                default:
                    {
                        LoggerAccessor.LogError(
                            $"[PREMIUMAGENCY] - Unhandled Server method: {method}"
                        );
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
                var regex = MyRegex();
                var matches = regex.Matches(formData);

                using (var writer = new StreamWriter(filePath))
                {
                    foreach (Match match in matches)
                    {
                        var key = match.Groups[1].Value.Trim();
                        var value = match.Groups[2].Value.Trim();

                        // Write key-value pair to the file
                        writer.WriteLine($"{key}: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"Fatal exception occured in WriteFormDataToFile with exception:\n",
                    ex
                );
            }
        }

        public static List<(string, string)> ReadFormDataFromFile(string filePath)
        {
            try
            {
                List<(string, string)> formData = [];

                using (var reader = new StreamReader(filePath))
                {
                    string line = null;
                    var currentKey = string.Empty;
                    var currentValue = string.Empty;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(':'))
                        {
                            var parts = line.Split([':'], 2, StringSplitOptions.RemoveEmptyEntries);
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
                LoggerAccessor.LogError(
                    $"Fatal exception occured in ReadFormDataFromFile with exception:\n",
                    ex
                );
                return null;
            }
        }

        [GeneratedRegex(@"name=""([^""]+)""\s*([\s\S]*?)\s*---------")]
        private static partial Regex MyRegex();
    }
}
