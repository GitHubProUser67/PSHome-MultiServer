using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using HttpMultipartParser;

namespace WebAPIService.VEEMEE.audi_vrun
{
    internal static class VrunScoreProcessor
    {
        private static object _Lock = new object();

        public static string SetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        float time = (float)double.Parse(data.GetParameterValue("time"), CultureInfo.InvariantCulture);
                        float dist = (float)double.Parse(data.GetParameterValue("dist"), CultureInfo.InvariantCulture);
                        string directoryPath = $"{apiPath}/VEEMEE/audi_vrun/User_Data";
                        string filePath = $"{directoryPath}/{psnid}.xml";
                        string highestScorePath = $"{directoryPath}/{psnid}_highest_score.txt";
                        int numOfRaces = 1;

                        Directory.CreateDirectory(directoryPath);

                        if (File.Exists(filePath))
                        {
                            // Load the XML string into an XmlDocument
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml($"<xml>{File.ReadAllText(filePath)}</xml>");

                            // Find the <distance> element
                            XmlElement xmlElement = xmlDoc.SelectSingleNode("/xml/scores/entry/distance") as XmlElement;

                            if (xmlElement != null)
                                // Replace the value of <distance> with a new value
                                xmlElement.InnerText = dist.ToString().Replace(",", ".");

                            // Find the <races> element
                            xmlElement = xmlDoc.SelectSingleNode("/xml/scores/entry/races") as XmlElement;

                            if (xmlElement != null)
                            {
                                numOfRaces = int.Parse(xmlElement.InnerText) + 1;
                                // Replace the value of <races> with a new value
                                xmlElement.InnerText = numOfRaces.ToString();
                            }

                            lock (_Lock)
                            {
                                if (File.Exists(highestScorePath))
                                {
                                    float currentTime = (float)double.Parse(File.ReadAllText(highestScorePath).Split(':')[0], CultureInfo.InvariantCulture);
                                    if (currentTime > time)
                                        File.WriteAllText(highestScorePath, time.ToString().Replace(",", ".") + $":{psnid}:{dist}");
                                }
                                else
                                    File.WriteAllText(highestScorePath, time.ToString().Replace(",", ".") + $":{psnid}:{dist}");
                            }

                            // Find the <time> element
                            xmlElement = xmlDoc.SelectSingleNode("/xml/scores/entry/time") as XmlElement;

                            if (xmlElement != null)
                                // Replace the value of <time> with a new value
                                xmlElement.InnerText = time.ToString().Replace(",", ".");

                            string XmlResult = xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
                            File.WriteAllText(filePath, XmlResult);
                            return XmlResult;
                        }
                        else
                        {
                            lock (_Lock)
                            {
                                if (File.Exists(highestScorePath))
                                {
                                    float currentTime = (float)double.Parse(File.ReadAllText(highestScorePath).Split(':')[0], CultureInfo.InvariantCulture);
                                    if (currentTime > time)
                                        File.WriteAllText(highestScorePath, time.ToString().Replace(",", ".") + $":{psnid}:{dist}");
                                }
                                else
                                    File.WriteAllText(highestScorePath, time.ToString().Replace(",", ".") + $":{psnid}:{dist}");
                            }

                            string XmlData = $"<scores><entry><psnid>{psnid}</psnid><races>1</races><distance>{dist.ToString().Replace(",", ".")}</distance><time>{time.ToString().Replace(",", ".")}</time></entry></scores>";
                            File.WriteAllText(filePath, XmlData);
                            return XmlData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - SetUserDataPOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        string directoryPath = $"{apiPath}/VEEMEE/audi_vrun/User_Data";
                        string filePath = $"{directoryPath}/{psnid}.xml";

                        if (File.Exists(filePath))
                            return File.ReadAllText(filePath);

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><distance>0</distance><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetUserDataPOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetHigherUserScorePOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        string directoryPath = $"{apiPath}/VEEMEE/audi_vrun/User_Data";
                        string filePath = $"{directoryPath}/{psnid}.xml";
                        string highestScorePath = $"{directoryPath}/{psnid}_highest_score.txt";

                        if (File.Exists(filePath) && File.Exists(highestScorePath))
                        {
                            string[] scoreData;
                            lock (_Lock)
                            {
                                scoreData = File.ReadAllText(highestScorePath).Split(':');
                            }
                            return Regex.Replace(File.ReadAllText(filePath), @"<distance>\d+(\.\d+)?</distance>", $"<distance>{scoreData[2]}</distance>");
                        }
                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><distance>0</distance><time>0</time></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        public static string GetGlobalTablePOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        string key = data.GetParameterValue("key");
                        if (key != "3Ebadrebr6qezag8")
                        {
                            CustomLogger.LoggerAccessor.LogError("[VEEMEE] - audi_vrun - Client tried to push invalid key! Invalidating request.");
                            return null;
                        }
                        string psnid = data.GetParameterValue("psnid");
                        string title = data.GetParameterValue("title");
                        return GenerateGlobalScoreXML(GetTopScores($"{apiPath}/VEEMEE/audi_vrun/User_Data"), title);
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError($"[VrunScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }

        private static string GenerateGlobalScoreXML(List<(string, float, string)> scores, string title)
        {
            if (scores.Count > 10)
                throw new InvalidDataException("[VrunScoreProcessor] - GenerateGlobalScoreXML received an invalid count of scores, only takes up to 10 entries.");

            int iY = 142; // Initial Y position
            StringBuilder data = new StringBuilder($"<XML><PAGE><RECT X=\"0\" Y=\"1\" W=\"0\" H=\"0\" col=\"#C0C0C0\" /><RECT X=\"0\" Y=\"0\" W=\"1280\" H=\"720\" col=\"#000000\" /><TEXT X=\"57\" Y=\"42\" col=\"#FFFFFF\" size=\"4\">{title}</TEXT>");

            for (int i = 0; i < scores.Count; i++)
            {
                data.AppendFormat("<RECT X=\"57\" Y=\"" + iY + "\" W=\"50\" H=\"50\" col=\"#662020\" />");
                data.AppendFormat("<RECT X=\"57\" Y=\"" + (iY + 45) + "\" W=\"50\" H=\"4\" col=\"#873030\" />");
                data.AppendFormat("<RECT X=\"973\" Y=\"" + iY + "\" W=\"254\" H=\"50\" col=\"#662020\" />");
                data.AppendFormat("<RECT X=\"973\" Y=\"" + (iY + 45) + "\" W=\"254\" H=\"4\" col=\"#873030\" />");
                data.AppendFormat("<RECT X=\"107\" Y=\"" + iY + "\" W=\"867\" H=\"50\" col=\"#313131\" />");
                data.AppendFormat("<RECT X=\"107\" Y=\"" + (iY + 45) + "\" W=\"867\" H=\"4\" col=\"#4D4D4D\" />");
                data.AppendFormat("<TEXT X=\"70\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, i + 1);

                data.AppendFormat("<TEXT X=\"190\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1}</TEXT>", iY + 5, scores[i].Item1);
                data.AppendFormat("<TEXT X=\"1015\" Y=\"{0}\" col=\"#FFFFFF\" size=\"3\">{1} m</TEXT>", iY + 5, scores[i].Item3);

                iY += 46; // Move down for next entry
            }

            data.Append("</PAGE></XML>");

            return data.ToString();
        }

        private static List<(string, float, string)> GetTopScores(string directoryPath)
        {
            List<(string, float, string)> scores = new List<(string, float, string)>();

            if (Directory.Exists(directoryPath))
            {
                lock (_Lock)
                {
                    foreach (string file in Directory.GetFiles(directoryPath, "*_highest_score.txt"))
                    {
                        try
                        {
                            string[] scoreData = File.ReadAllText(file).Split(':');
                            if (scoreData.Length == 3)
                                scores.Add((scoreData[1], (float)double.Parse(scoreData[0], CultureInfo.InvariantCulture), scoreData[2]));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return scores.OrderByDescending(s => s.Item2).Take(10).ToList();
        }
    }
}
