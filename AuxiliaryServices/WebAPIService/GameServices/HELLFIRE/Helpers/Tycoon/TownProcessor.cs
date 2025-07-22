using CustomLogger;
using HttpMultipartParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebAPIService.GameServices.HELLFIRE.Helpers.Tycoon
{
    public class TownProcessor
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
                    ms.Flush();
                }

                string filePath = $"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{TownID}.xml";
                DateTime CurrentTime = DateTime.Now;

                if (File.Exists(filePath))
                {
                    File.WriteAllText(
                        filePath,
                        Regex.Replace(
                            File.ReadAllText(filePath),
                            $@"<{Index}>(<TimeBuilt>.*?</TimeBuilt>)(<Orientation>.*?</Orientation>)(<Index>{Index}</Index>)(<Type>.*?</Type>)</{Index}>",
                            $@"<{Index}><TimeBuilt>{CurrentTime}</TimeBuilt><Orientation>{Orientation}</Orientation><Index>{Index}</Index><Type>{Type}</Type></{Index}>",
                            RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace));
                }


                return $"<Response><TimeBuilt>{CurrentTime}</TimeBuilt><Orientation>{Orientation}</Orientation><Index>{Index}</Index><Type>{Type}</Type></Response>";
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
                    ms.Flush();
                }

                string filePath = $"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{TownID}.xml";

                if (File.Exists(filePath))
                {
                    List<BuildingData> buildingDataList = JsonConvert.DeserializeObject<List<BuildingData>>(BuildingDataEncoded);

                    foreach (var BuildingData in buildingDataList)
                    {
                        string BuildingIndex = BuildingData.Index;
                        string bIdxAppend = BuildingIndex + ".000000";
                        string pattern = $@"<({bIdxAppend})>.*?<TimeBuilt>(.*?)</TimeBuilt>.*?<Orientation>(.*?)</Orientation>.*?<Index>(.*?)</Index>.*?<Type>(.*?)</Type>.*?</{bIdxAppend}>";

                        string userTown = File.ReadAllText(filePath);
                        var match = Regex.Match(userTown, pattern, RegexOptions.Singleline);

                        if (match.Success)
                        {
                            string tileIndex = match.Groups[1].Value;
                            string timeBuilt = match.Groups[2].Value;
                            string orientation = match.Groups[3].Value;
                            string index = match.Groups[4].Value;
                            string type = match.Groups[5].Value;

                            // Build the updated XML
                            string updatedXml = $@"<{tileIndex}><TimeBuilt>{timeBuilt}</TimeBuilt><Orientation>{orientation}</Orientation><Index>{index}</Index><Type>{type}</Type><WorkersSpent>{BuildingData.WorkersSpent}</WorkersSpent><Money>{BuildingData.Money}</Money><Population>{BuildingData.Population}</Population></{tileIndex}>";

                            File.WriteAllText(filePath, userTown.Replace(match.Value, updatedXml));
                        }
                        else
                            LoggerAccessor.LogWarn($"[TownProcessor] - No building match found for file: {filePath}.");
                    }
                }

                return "<Response></Response>";
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

                if (File.Exists($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{TownID}.xml"))
                    File.WriteAllText($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{TownID}.xml",
                    Regex.Replace(File.ReadAllText($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{TownID}.xml"),
                    $"<{BuildingIndex:F6}>(<TimeBuilt>).*?(</TimeBuilt>)(<Orientation>).*?(</Orientation>)(<Index>){BuildingIndex:F6}(</Index>)(<Type>).*?(</Type>)</{BuildingIndex:F6}>",
                    $"<{BuildingIndex:F6}><TimeBuilt></TimeBuilt><Orientation></Orientation><Index>{BuildingIndex}</Index><Type></Type></{BuildingIndex:F6}>"));
            }

            return "<Response></Response>";
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
                ms.Flush();
            }

            // Retrieve the user's JSON profile
            string profilePath = $"{WorkPath}/TYCOON/User_Data/{UserID}/TownVisitors_{TownID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<xml>" + xmlProfile + "</xml>");
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data

                        switch (cmd)
                        {
                            case "AddVisitor":
                                {
                                    XmlElement VisitorElement = doc.CreateElement(VisitorID);
                                    VisitorElement.InnerText = VisitorID;
                                    doc.DocumentElement.AppendChild(VisitorElement);
                                }
                                break;
                            case "GetVisitors":
                                {
                                    // Get the current UTC time as unix timestamp for LastCollectionTime
                                    long unixTimestamp = new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds();

                                    xmlResponse = $"<Response><LastCollectionTime>{unixTimestamp}</LastCollectionTime>{xmlProfile}</Response>";
                                }
                                break;
                            case "ClearVisitors":
                                {
                                    doc.DocumentElement.RemoveAll();
                                }
                                break;
                        }



                        // Get the updated XML string
                        string updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
                        // Save the updated profile back to the file
                        File.WriteAllText(profilePath, updatedXMLProfile);
                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HFGAMES] User - An assertion was thrown in UpdateUser : {ex}");
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