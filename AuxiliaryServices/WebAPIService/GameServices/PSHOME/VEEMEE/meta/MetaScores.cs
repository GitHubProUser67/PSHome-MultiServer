using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.meta
{
    public class MetaScores
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string game_id = string.Empty;
            string sort_1 = string.Empty;
            string sort_2 = string.Empty;
            string score_1 = string.Empty;
            string score_2 = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "JPDFC10A9MXS8HHOMOUKYAR3")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();
                game_id = data["game_id"].First();
                sort_1 = data["sort_1"].First();
                sort_2 = data["sort_2"].First();
                score_1 = data["score_1"].First();
                score_2 = data["score_2"].First();

                string directoryPath = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_1}/User_Data";
                string directoryPath_2 = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_2}/User_Data";

                string filePath = $"{directoryPath}/{psnid}.xml";
                string filePath_2 = $"{directoryPath}/{psnid}_2.xml";

                Directory.CreateDirectory(directoryPath);
                Directory.CreateDirectory(directoryPath_2);

                if (File.Exists(filePath) && File.Exists(filePath_2))
                {
                    // Load the XML string into an XmlDocument
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml($"<xml>{File.ReadAllText(filePath)}</xml>");

                    // Find the <score> element
                    XmlElement scoreElement = xmlDoc.SelectSingleNode("/xml/score") as XmlElement;

                    if (scoreElement != null)
                        // Replace the value of <score> with a new value
                        scoreElement.InnerText = score_1;

                    File.WriteAllText(filePath, xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty));

                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml($"<xml>{File.ReadAllText(filePath_2)}</xml>");

                    // Find the <score> element
                    scoreElement = xmlDoc.SelectSingleNode("/xml/score") as XmlElement;

                    if (scoreElement != null)
                        // Replace the value of <score> with a new value
                        scoreElement.InnerText = score_2;

                    File.WriteAllText(filePath, xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty));

                    return $"<score><player><psn>{psnid}</psn><score_1>{score_1}</score_1><score_2>{score_2}</score_2></player></score>";
                }
                else
                {
                    File.WriteAllText(filePath, $"<score>{score_1}</score>");
                    File.WriteAllText(filePath_2, $"<score>{score_2}</score>");

                    return $"<score><player><psn>{psnid}</psn><score_1>{score_1}</score_1><score_2>{score_2}</score_2></player></score>";
                }
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string game_id = string.Empty;
            string sort_1 = string.Empty;
            string sort_2 = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "JPDFC10A9MXS8HHOMOUKYAR3")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - meta_scores - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();
                game_id = data["game_id"].First();
                sort_1 = data["sort_1"].First();
                sort_2 = data["sort_2"].First();

                string directoryPath = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_1}/User_Data";
                string directoryPath_2 = $"{apiPath}/VEEMEE/meta_scores/{game_id}/{sort_2}/User_Data";

                string filePath = $"{directoryPath}/{psnid}.xml";
                string filePath_2 = $"{directoryPath}/{psnid}_2.xml";

                if (File.Exists(filePath) && File.Exists(filePath_2))
                {
                    string score_1 = File.ReadAllText(filePath).Replace("<score>", string.Empty).Replace("</score>", string.Empty);
                    string score_2 = File.ReadAllText(filePath_2).Replace("<score>", string.Empty).Replace("</score>", string.Empty);
                    return $"<score><player><psn>{psnid}</psn><score_1>{score_1}</score_1><score_2>{score_2}</score_2></player></score>";
                }
            }

            return $"<score><player><psn>{psnid}</psn><score_1>0</score_1><score_2>0</score_2></player></score>";
        }
    }
}
