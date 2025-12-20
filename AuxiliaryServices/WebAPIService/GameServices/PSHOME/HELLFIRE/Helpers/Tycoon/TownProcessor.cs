using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.HomeTycoon;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Tycoon
{
    internal class TownProcessor
    {
        public static string CreateBuilding(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string Orientation = string.Empty;
            string Type = string.Empty;
            string TownID = string.Empty;
            string Index = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    Orientation = data.GetParameterValue("Orientation");
                    Type = data.GetParameterValue("Type");
                    TownID = data.GetParameterValue("TownID");
                    Index = data.GetParameterValue("Index");
                }

                string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";

                if (File.Exists(filePath))
                {
                    string pattern = $@"<{Index}>(.*?)</{Index}>";

                    string userTown = File.ReadAllText(filePath);
                    var match = Regex.Match(userTown, pattern, RegexOptions.Singleline);

                    if (match.Success)
                    {
                        long CurrentTime = DateTimeUtils.GetUnixTime();

                        if (match.Groups[1].Value == "0")
                            userTown = userTown.Replace(match.Value, $@"<{Index}><TimeBuilt>{CurrentTime}</TimeBuilt><Orientation>{Orientation}</Orientation><Index>{Index}</Index><Type>{Type}</Type></{Index}>");
                        else
                        {
                            userTown = Regex.Replace(
                            userTown,
                            $@"<{Index}>(<TimeBuilt>.*?</TimeBuilt>)(<Orientation>.*?</Orientation>)(<Index>{Index}</Index>)(<Type>.*?</Type>).*?</{Index}>",
                            $@"<{Index}><TimeBuilt>{CurrentTime}</TimeBuilt><Orientation>{Orientation}</Orientation><Index>{Index}</Index><Type>{Type}</Type></{Index}>",
                            RegexOptions.Singleline);
                        }

                        File.WriteAllText(filePath, userTown);

                        return $"<Response><TimeBuilt>{CurrentTime}</TimeBuilt><Orientation>{Orientation}</Orientation><Index>{Index}</Index><Type>{Type}</Type></Response>";
                    }
                }
            }

            return "<Response></Response>";
        }

        public static string UpdateBuildings(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string TownID = string.Empty;
            string BuildingDataEncoded = string.Empty;
            string TotalPopulation = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    TownID = data.GetParameterValue("TownID");
                    BuildingDataEncoded = data.GetParameterValue("BuildingData");
                    TotalPopulation = data.GetParameterValue("TotalPopulation");
                }

                string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";

                if (File.Exists(filePath))
                {
                    string userTown = File.ReadAllText(filePath);

                    foreach (var BuildingData in JsonConvert.DeserializeObject<List<BuildingData>>(BuildingDataEncoded))
                    {
                        string bIdx = BuildingData.Index + ".000000";

                        var match = Regex.Match(userTown, $@"<{bIdx}>(.*?)</{bIdx}>", RegexOptions.Singleline);

                        if (!match.Success)
                        {
                            LoggerAccessor.LogWarn($"[TownProcessor] - No building match found for index {bIdx} in file {filePath}.");
                            continue;
                        }

                        string innerContent = match.Groups[1].Value.Trim();

                        string timeBuilt = "0";
                        string orientation = "0";
                        string type = BuildingData.Type;

                        if (innerContent != "0")
                        {
                            timeBuilt = Regex.Match(innerContent, @"<TimeBuilt>(.*?)</TimeBuilt>").Groups[1].Value;
                            orientation = Regex.Match(innerContent, @"<Orientation>(.*?)</Orientation>").Groups[1].Value;

                            var typeMatch = Regex.Match(innerContent, @"<Type>(.*?)</Type>");
                            if (typeMatch.Success)
                                type = string.IsNullOrEmpty(type) ? typeMatch.Groups[1].Value : type;
                        }

                        userTown = userTown.Replace(match.Value, $"<{bIdx}>" +
                            $"<TimeBuilt>{timeBuilt}</TimeBuilt>" +
                            $"<Orientation>{orientation}</Orientation>" +
                            $"<Index>{bIdx}</Index>" +
                            $"<Type>{type}</Type>" +
                            $"<WorkersSpent>{BuildingData.WorkersSpent}</WorkersSpent>" +
                            $"<Money>{BuildingData.Money.ToString().Replace(",", ".")}</Money>" +
                            $"<Population>{BuildingData.Population.ToString().Replace(",", ".")}</Population>" +
                            $"</{bIdx}>");
                    }

                    File.WriteAllText(filePath, userTown);
                }
            }

            return "<Response></Response>";
        }

        public static string RemoveBuilding(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string xmlprofile = string.Empty;
            string TownID = string.Empty;
            string BuildingIndex = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    TownID = data.GetParameterValue("TownID");
                    BuildingIndex = data.GetParameterValue("BuildingIndex");
                    ms.Flush();
                }

                string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";

                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath,
                        Regex.Replace(File.ReadAllText(filePath),
                        $"<{BuildingIndex:F6}>(.*?)</{BuildingIndex:F6}>",
                        $"<{BuildingIndex:F6}>0</{BuildingIndex:F6}>"));
                }
            }

            return "<Response></Response>";
        }

        public static string UpdateTownTime(string UserID, string TownID, string WorkPath)
        {
            string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";
            string xml = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

            const string pattern = @"<LastVisited>.*?</LastVisited>";

            if (Regex.IsMatch(xml, pattern))
                xml = Regex.Replace(
                        xml,
                        pattern,
                        $"<LastVisited>{DateTimeUtils.GetUnixTime()}</LastVisited>"
                    );

            File.WriteAllText(filePath, xml);

            return "<Response></Response>";
        }

        // This one updates the timestamp on the "LastVisited" row every few minutes
        // and also keeps the number of people in the city updated
        public static string UpdateTownPlayers(string UserID, string TownID, string NumPlayers, string WorkPath)
        {
            UpdateTownTime(UserID, TownID, WorkPath);

            string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";
            string xml = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

            const string pattern = @"<NumPlayers>.*?</NumPlayers>";

            if (Regex.IsMatch(xml, pattern))
                xml = Regex.Replace(
                        xml,
                        pattern,
                        $"<NumPlayers>{NumPlayers}</NumPlayers>"
                    );

            File.WriteAllText(filePath, xml);

            return "<Response></Response>";
        }

        public static string GetTownPlayers(string UserID, string TownID, string WorkPath)
        {
            string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";

            if (File.Exists(filePath))
            {
                const string pattern = @"<NumPlayers>(.*?)</NumPlayers>";

                Match match = Regex.Match(File.ReadAllText(filePath), pattern);

                if (match.Success)
                    return match.Groups[1].Value;
            }

            return "0";
        }

        public static void UpdateTownPrivacy(string UserID, string TownID, TycoonPrivacySetting NewSetting, string WorkPath)
        {
            UpdateTownTime(UserID, TownID, WorkPath);

            string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";
            string xml = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

            const string pattern = @"<Privacy>.*?</Privacy>";

            if (Regex.IsMatch(xml, pattern))
                xml = Regex.Replace(
                        xml,
                        pattern,
                        $"<Privacy>{(int)NewSetting}</Privacy>"
                    );

            File.WriteAllText(filePath, xml);
        }

        public static TycoonPrivacySetting GetTownPrivacy(string UserID, string TownID, string WorkPath)
        {
            string filePath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/{TownID}.xml";

            if (File.Exists(filePath))
            {
                const string pattern = @"<Privacy>(.*?)</Privacy>";

                Match match = Regex.Match(File.ReadAllText(filePath), pattern);

                if (match.Success)
                    return (TycoonPrivacySetting)int.Parse(match.Groups[1].Value);
            }

            return TycoonPrivacySetting.Public;
        }

        public static string HandleVisitors(byte[] PostData, string boundary, string UserID, string WorkPath, string cmd)
        {
            string TownID = string.Empty;
            string VisitorID = string.Empty;
            string xmlProfile = string.Empty;
            string xmlResponse = "<Response></Response>";

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);
                TownID = data.GetParameterValue("TownID");
                VisitorID = data.GetParameterValue("VisitorID");
            }

            string townVisitorsPath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/TownVisitors_{TownID}.xml";

            if (File.Exists(townVisitorsPath))
                xmlProfile = File.ReadAllText(townVisitorsPath);

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<xml>" + xmlProfile + "</xml>");

                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);

                        switch (cmd)
                        {
                            case "AddVisitor":
                                {
                                    // Allow duplicates for RPCN.
                                    XmlElement VisitorElement = doc.CreateElement(VisitorID);
                                    VisitorElement.InnerText = VisitorID;
                                    doc.DocumentElement.AppendChild(VisitorElement);
                                }
                                break;
                            case "GetVisitors":
                                {
                                    // Get the current UTC time as unix timestamp for LastCollectionTime
                                    xmlResponse = $"<Response><LastCollectionTime>{new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds()}</LastCollectionTime>{xmlProfile}</Response>";
                                }
                                break;
                            case "ClearVisitors":
                                {
                                    doc.DocumentElement.RemoveAll();
                                }
                                break;
                        }

                        File.WriteAllText(townVisitorsPath, doc.DocumentElement.InnerXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HandleVisitors] - HandleVisitors: An assertion was thrown. (Exception:{ex})");
            }

            return xmlResponse;
        }
    }
}

public class BuildingData
{
    public double Money { get; set; }
    public double Population { get; set; }
    public string Type { get; set; }
    public string Index { get; set; }
    public int WorkersSpent { get; set; }
}