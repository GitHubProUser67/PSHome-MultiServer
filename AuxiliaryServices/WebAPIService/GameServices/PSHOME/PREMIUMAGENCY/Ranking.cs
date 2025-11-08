using System.IO;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Text;


#if !NETFRAMEWORK
using System.Web;
#endif
namespace WebAPIService.GameServices.PSHOME.PREMIUMAGENCY
{
    public class Ranking
    {
        public static string getItemRankingTableHandler(byte[] PostData, string ContentType, string workPath, string eventId, string fulluripath, string method)
        {
            string nid = string.Empty;

            if (method == "GET")
#if NETFRAMEWORK
                nid = HTTPProcessor.GetQueryParameters(fulluripath)["nid"];
#else
                nid = HttpUtility.ParseQueryString(fulluripath).Get("nid");
#endif
            else
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    nid = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("nid");

                    ms.Flush();
                }
            }

            if (nid == null || eventId == null)
            {
                LoggerAccessor.LogError($"[PREMIUMAGENCY] - name id {nid} or eventid {eventId} is null, this shouldn't happen!!!");
                return null;
            }

            #region Paths

            string homeSquareT037Path = $"{workPath}/eventController/ItemRankings/hs/T037/";

            string JapanChristmas2010 = $"{workPath}/eventController/Christmas/2010/ItemRankings/";


            string MikuLiveJukeboxPath = $"{workPath}/eventController/MikuLiveJukebox";
            string j_liargame2Path = $"{workPath}/eventController/j_liargame2/ItemRankings/";
            #endregion

            switch (eventId)
            {
                case "86":
                    {
                        Directory.CreateDirectory(homeSquareT037Path);
                        string filePath = $"{homeSquareT037Path}/getItemRankingTable.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC HomeSquare T037 {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"{res}\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC HomeSquare T037 {eventId}! Expected path {filePath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }
                case "92":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/getItemRankingTable.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC MikuLiveJukebox {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"{res}\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC MikuLiveJukebox {eventId}! Expected path {MikuLiveJukeboxPath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\t" +
                                 "</xml>";
                        }
                    }
                case "299":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/getItemRankingTable.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC LiarGame2 {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"{res}\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC LiarGame2 {eventId}! Expected path {filePath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }
                case "310":
                    {
                        Directory.CreateDirectory(JapanChristmas2010);
                        string filePath = $"{JapanChristmas2010}/getItemRankingTable.xml";

                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC Japan Christmas 2010 {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return res;
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC Japan Christmas 2010 {eventId}! Expected path {filePath}");
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }
                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - GetItemRankingTable unhandled for eventId {eventId} | POSTDATA: \n{PostData}");
                        return null;
                    }
            }
        }

        public static string entryItemRankingPointsHandler(byte[] PostData, string ContentType, string workPath, string eventId, string fulluripath, string method)
        {
            string nid = string.Empty;

            if (method == "GET")
#if NETFRAMEWORK
                nid = HTTPProcessor.GetQueryParameters(fulluripath)["nid"];
#else
                nid = HttpUtility.ParseQueryString(fulluripath).Get("nid");
#endif
            else
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    nid = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("nid");

                    ms.Flush();
                }
            }

            if (nid == null || eventId == null)
            {
                LoggerAccessor.LogError($"[PREMIUMAGENCY] - name id {nid} or eventid {eventId} is null, this shouldn't happen!!!");
                return null;
            }

            #region Paths

            string homeSquareT037Path = $"{workPath}/eventController/ItemRankings/hs/T037/ItemRankings";


            string MikuLiveJukeboxPath = $"{workPath}/eventController/MikuLiveJukebox/ItemRankings";
            string j_liargame2Path = $"{workPath}/eventController/j_liargame2/ItemRankings";
            #endregion

            switch (eventId)
            {
                case "86":
                    {
                        Directory.CreateDirectory(homeSquareT037Path);
                        string filePath = $"{homeSquareT037Path}/{nid}.cache";
                        PREMIUMAGENCYClass.WriteFormDataToFile(Encoding.UTF8.GetString(PostData), filePath);
                        LoggerAccessor.LogInfo("[PREMIUMAGENCY] - FormData written to file: " + filePath);
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FOUND for PUBLIC HomeSquare T037 {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FALLBACK sent for PUBLIC HomeSquare T037 {eventId}! Expected path {filePath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }
                case "92":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{nid}.cache";
                        PREMIUMAGENCYClass.WriteFormDataToFile(Encoding.UTF8.GetString(PostData), filePath);
                        LoggerAccessor.LogInfo("[PREMIUMAGENCY] - FormData written to file: " + filePath);
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FOUND for PUBLIC MikuliveJukebox {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FALLBACK sent for PUBLIC MikuliveJukebox {eventId}! Expected path {filePath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }

                case "299":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{nid}.cache";
                        PREMIUMAGENCYClass.WriteFormDataToFile(Encoding.UTF8.GetString(PostData), filePath);
                        LoggerAccessor.LogInfo("[PREMIUMAGENCY] - FormData written to file: " + filePath);
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FOUND for PUBLIC LiarGame2 {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - EntryItemRankingPoints FALLBACK sent for PUBLIC LiarGame2 {eventId}! Expected path {filePath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"<status type=\"int\">0</status>\r\n" +
                                 "</xml>";
                        }
                    }
                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - EntryItemRankingPoints unhandled for eventId {eventId} | POSTDATA: \n{Encoding.UTF8.GetString(PostData)}");
                        return null;
                    }
            }
        }


        public static string getItemRankingTargetListHandler(byte[] PostData, string ContentType, string workPath, string eventId, string fulluripath, string method)
        {
            string nid = string.Empty;

            if (method == "GET")
#if NETFRAMEWORK
                nid = HTTPProcessor.GetQueryParameters(fulluripath)["nid"];
#else
                nid = HttpUtility.ParseQueryString(fulluripath).Get("nid");
#endif
            else
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    nid = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("nid");

                    ms.Flush();
                }
            }

            if (nid == null || eventId == null)
            {
                LoggerAccessor.LogError("[PREMIUMAGENCY] - name id or event id is null, this shouldn't happen!!!");
                return null;
            }

            #region Paths


            string MikuLiveJukeboxPath = $"{workPath}/eventController/MikuLiveJukebox";
            string T037HomeSquare = $"{workPath}/eventController/ItemRankings/hs/T037";
            #endregion

            switch (eventId)
            {
                case "92":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/getItemRankingTargetList.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC MikuLiveJukebox {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"{res}\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC MikuLiveJukebox {eventId}! Expected path {MikuLiveJukeboxPath}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\t" +
                                 "</xml>";
                        }
                    }
                case "86":
                    {
                        Directory.CreateDirectory(T037HomeSquare);
                        string filePath = $"{T037HomeSquare}/getItemRankingTargetList.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FOUND for PUBLIC T037HomeSquare {eventId}!");
                            string res = File.ReadAllText(filePath);
                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                                 $"{res}\r\n" +
                                 "</xml>";
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetItemRankingTable FALLBACK sent for PUBLIC T037HomeSquare {eventId}! Expected path {T037HomeSquare}");

                            return "<xml>\r\n\t" +
                                 "<result type=\"int\">1</result>\r\n\t" +
                                 "<description type=\"text\">Success</description>\r\n\t" +
                                 "<error_no type=\"int\">0</error_no>\r\n\t" +
                                 "<error_message type=\"text\">None</error_message>\r\n\t" +
                                 "</xml>";
                        }
                    }
                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - GetItemRankingTable unhandled for eventId {eventId} | POSTDATA: \n{PostData}");
                        return null;
                    }
            }
        }




    }
}
