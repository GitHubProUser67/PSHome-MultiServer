using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.VEEMEE.goalie_sfrgbt
{
    public class UserData
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, bool global, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string guest = string.Empty;
            string goals = string.Empty;
            string duration = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "d2us7A2EcU2PuBuz")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - goalie_sfrgbt - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();
                guest = data["guest"].First();
                goals = data["goals"].First();
                duration = data["duration"].First();

                string directoryPath = string.Empty;
                string filePath = string.Empty;

                if (global)
                {
                    directoryPath = $"{apiPath}/VEEMEE/goalie/User_Data";
                    filePath = $"{apiPath}/VEEMEE/goalie/User_Data/{psnid}.xml";
                }
                else
                {
                    directoryPath = $"{apiPath}/VEEMEE/sfrgbt/User_Data";
                    filePath = $"{apiPath}/VEEMEE/sfrgbt/User_Data/{psnid}.xml";
                }

                Directory.CreateDirectory(directoryPath);

                if (File.Exists(filePath))
                {
                    try
                    {
                        GSScoreBoardData.UpdateScoreBoard(psnid, duration, (float)double.Parse(goals, CultureInfo.InvariantCulture));
                        GSScoreBoardData.UpdateAllTimeScoreboardXml(apiPath, global); // We finalized edit, so we issue a write.
                        GSScoreBoardData.UpdateTodayScoreboardXml(apiPath, global, DateTime.Now.ToString("yyyy_MM_dd")); // We finalized edit, so we issue a write.
                    }
                    catch (Exception)
                    {
                        // Not Important
                    }

                    string XmlData = File.ReadAllText(filePath);

                    // Check for invalid profiles.
                    if (XmlData.StartsWith("<scores></scores>"))
                    {
                        XmlData = $"<scores><entry><psnid>{psnid}</psnid><goals>{goals}</goals><duration>{duration}</duration><paid_goals></paid_goals></entry></scores>";
                        File.WriteAllText(filePath, XmlData);
                        return XmlData;
                    }

                    // Load the XML string into an XmlDocument
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml($"<xml>{File.ReadAllText(filePath)}</xml>");

                    // Find the <goals> element
                    XmlElement goalsElement = xmlDoc.SelectSingleNode("/xml/scores/entry/goals") as XmlElement;

                    if (goalsElement != null)
                    {
                        // Replace the value of <goals> with a new value
                        goalsElement.InnerText = goals;

                        // Find the <duration> element
                        XmlElement durationElement = xmlDoc.SelectSingleNode("/xml/scores/entry/duration") as XmlElement;

                        if (durationElement != null)
                        {
                            // Replace the value of <duration> with a new value
                            durationElement.InnerText = duration;

                            string XmlResult = xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
                            File.WriteAllText(filePath, XmlResult);
                            return XmlResult;
                        }
                    }
                }
                else
                {
                    string XmlData = $"<scores><entry><psnid>{psnid}</psnid><goals>{goals}</goals><duration>{duration}</duration><paid_goals></paid_goals></entry></scores>";
                    File.WriteAllText(filePath, XmlData);
                    return XmlData;
                }
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string ContentType, bool global, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "d2us7A2EcU2PuBuz")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - goalie_sfrgbt - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                string directoryPath = string.Empty;
                string filePath = string.Empty;

                if (global)
                {
                    directoryPath = $"{apiPath}/VEEMEE/goalie/User_Data";
                    filePath = $"{apiPath}/VEEMEE/goalie/User_Data/{psnid}.xml";
                }
                else
                {
                    directoryPath = $"{apiPath}/VEEMEE/sfrgbt/User_Data";
                    filePath = $"{apiPath}/VEEMEE/sfrgbt/User_Data/{psnid}.xml";
                }

                if (File.Exists(filePath))
                {
                    string XmlData = File.ReadAllText(filePath);

                    // Check for invalid profiles.
                    if (!XmlData.StartsWith("<scores></scores>"))
                        return XmlData;
                }
            }

            return $"<scores><entry><psnid>{psnid}</psnid><goals>0</goals><duration>0</duration><paid_goals></paid_goals></entry></scores>";
        }
    }
}
