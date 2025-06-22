using NetworkLibrary.HTTP;
using HttpMultipartParser;
using System.IO;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WebAPIService.NDREAMS.Xi2
{
    public class Cont
    {
        public const string ContSignature = "nDreamsXi2Cont";

        private static readonly Dictionary<int, string> dayDictionary = new Dictionary<int, string>
        {
            { -1, "debug" },
            { 0, "day 0" },
            { 1, "day 1" },
            { 2, "day 2" },
            { 3, "day 3" },
            { 4, "day 4" },
            { 5, "day 5" },
            { 6, "day 6" },
            { 7, "day 7" },
            { 8, "day 8" },
            { 9, "day 9" },
            { 10, "day 10" },
            { 11, "day 11" },
            { 12, "day 12" },
            { 13, "day 13" },
            { 14, "day 14" },
            { 15, "day 15" },
            { 16, "day 16" },
            { 17, "day 17" },
            { 18, "day 18" },
            { 19, "day 19" },
            { 20, "day 20" },
            { 21, "day 21" },
            { 22, "day 22" },
            { 23, "day 23" },
            { 24, "day 24" },
            { 25, "day 25" },
            { 26, "day 26" },
            { 27, "day 27" },
            { 28, "day 28" },
            { 29, "day 29" },
            { 30, "day 30" },
            { 31, "day 31" },
            { 32, "day 32" },
            { 33, "day 33" },
            { 34, "day 34" },
            { 35, "day 35" },
            { 36, "day 36" },
            { 37, "day 37" },
            { 38, "day 38" },
            { 39, "day 39" },
            { 40, "day 40" },
            { 41, "day 41" },
            { 42, "day 42" },
            { 43, "day 43" },
            { 44, "day 44" },
            { 45, "day 45" },
            { 46, "day 46" },
            { 47, "day 47" },
            { 48, "Well Done!" }
        };

        private static string FormatDayDictionary()
        {
            StringBuilder formattedString = new StringBuilder();

            foreach (var kvp in dayDictionary)
            {
                formattedString.AppendFormat("<name>{0}</name><id>{1}</id>", kvp.Value, kvp.Key);
            }

            return formattedString.ToString();
        }

        // Function to retrieve the index by day value
        private static int? GetDayIndexByValue(string dayValue)
        {
            return dayDictionary.FirstOrDefault(x => x.Value == dayValue).Key;
        }

        // Function to retrieve the day value by index
        private static string GetDayValueByIndex(int index)
        {
            int minIndex = dayDictionary.Keys.Min();

            if (index < minIndex) // Wrap up to debug.
                index = minIndex;
            else if (index > dayDictionary.Keys.Max()) // Wrap up to new game+
                index = 0;

            return dayDictionary.TryGetValue(index, out string value) ? value : string.Empty;
        }

        public static string ProcessCont(DateTime CurrentDate, byte[] PostData, string ContentType, string apipath)
        {
            string func = null;
            string key = null;
            string ExpectedHash = null;
            string name = null;
            string region = null;
            string territory = null;
            string progress = null;
            string stats = null;
            string hash = null;
            string day = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");
                    try
                    {
                        name = data.GetParameterValue("name");
                    }
                    catch
                    {
                    }

                    if (!string.IsNullOrEmpty(func))
                    {
                        string directoryPath = apipath + $"/NDREAMS/Xi2/PlayersInventory/{name}";
                        string profilePath = directoryPath + "/Cont.xml";

                        switch (func)
                        {
                            case "xoff":
                                territory = data.GetParameterValue("territory");
                                day = data.GetParameterValue("day");

                                ExpectedHash = Aurora.Teaser.Xoff_VerifyKey(territory, day);

                                if (key == ExpectedHash)
                                {
                                    byte MockedDay = 0;

                                    // Get the current day of the week
                                    switch (DateTime.Today.DayOfWeek)
                                    {
                                        case DayOfWeek.Monday:
                                            MockedDay = 5;
                                            break;
                                        case DayOfWeek.Tuesday:
                                            MockedDay = 4;
                                            break;
                                        case DayOfWeek.Wednesday:
                                            MockedDay = 3;
                                            break;
                                        case DayOfWeek.Thursday:
                                            MockedDay = 2;
                                            break;
                                        case DayOfWeek.Friday:
                                            MockedDay = 1;
                                            break;
                                    }

                                    return $"<xml><success>true</success><result><day>{MockedDay}</day><hash>{Aurora.Teaser.Xoff_GetSignature(int.Parse(day), MockedDay)}</hash></result></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Xoff: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "init":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    region = data.GetParameterValue("region");
                                    territory = data.GetParameterValue("territory");

                                    ContProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = ContProfileData.DeserializeProfileData(profilePath);
                                    else
                                    {
                                        profileData = new ContProfileData() { GameDay = "day 0", NextDay = "day 1", DayIdx = 0, UserType = 0, Stats = "AQAAAAAAAAAAAFAAAAAAAAA=", Hash = "11bf01dab261092b40969af7c4e2a6c5acc15f11" };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><result><GameDay>{profileData.GameDay}</GameDay><GameDayProgress>{profileData.GameDayProgress}</GameDayProgress><NextDay>{profileData.NextDay}</NextDay>" +
                                        $"<Idx>{profileData.DayIdx}</Idx><UTId>{profileData.UserType}</UTId><Hash>{profileData.Hash}</Hash><Stats>{profileData.Stats}</Stats>" +
                                        $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, $"{profileData.GameDay}{profileData.GameDayProgress}{profileData.Stats}", CurrentDate)}</confirm></result></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "update_progress":
                                progress = data.GetParameterValue("progress");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func + progress, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ContProfileData profileData = ContProfileData.DeserializeProfileData(profilePath);
                                        profileData.GameDayProgress = progress;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - Cont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "update_stats":
                                stats = data.GetParameterValue("stats");
                                hash = data.GetParameterValue("hash");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func + hash + stats, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ContProfileData profileData = ContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Stats = stats;
                                        profileData.Hash = hash;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - Cont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "next_day":
                                day = data.GetParameterValue("day");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func + day, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    string errMsg = $"[Xi2] - Cont: Profile doesn't exist!";

                                    if (File.Exists(profilePath))
                                    {
                                        int? dayIdx = GetDayIndexByValue(day);

                                        if (dayIdx.HasValue)
                                        {
                                            ContProfileData profileData = ContProfileData.DeserializeProfileData(profilePath);
                                            profileData.GameDay = day;
                                            profileData.GameDayProgress = string.Empty;
                                            profileData.DayIdx = dayIdx.Value;
                                            profileData.NextDay = GetDayValueByIndex(dayIdx.Value + 1);

                                            profileData.SerializeProfileData(profilePath);

                                            return $"<xml><success>true</success><result><Success>true</Success><GameDay>{profileData.GameDay}</GameDay><GameDayProgress>{profileData.GameDayProgress}</GameDayProgress><NextDay>{profileData.NextDay}</NextDay>" +
                                                $"<Idx>{profileData.DayIdx}</Idx><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, $"{profileData.GameDay}{profileData.GameDayProgress}", CurrentDate)}</confirm></result></xml>";
                                        }
                                        else
                                        {
                                            errMsg = $"[Xi2] - Cont: Requested day is invalid!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><success>false</success><error>Invalid day</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                        }
                                    }

                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "eta_next_day":
                                day = data.GetParameterValue("day");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func + day, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    int? dayIdx = GetDayIndexByValue(day);

                                    if (dayIdx.HasValue)
                                        return $"<xml><success>true</success><result><Success>true</Success><Available>true</Available><TooLong>false</TooLong><Mins>00</Mins><Hours>00</Hours><Days>00</Days><Next>{GetDayValueByIndex(dayIdx.Value + 1)}</Next>" +
                                                $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, $"{GetDayValueByIndex(dayIdx.Value + 1)}00", CurrentDate)}</confirm></result></xml>";
                                    else
                                    {
                                        string errMsg = $"[Xi2] - Cont: Requested day is invalid!";
                                        CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                        return $"<xml><success>false</success><error>Invalid day</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                    }
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "debug_set_day":
                                day = data.GetParameterValue("day");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func + day, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    string errMsg = $"[Xi2] - Cont: Profile doesn't exist!";

                                    if (File.Exists(profilePath))
                                    {
                                        int? dayIdx = GetDayIndexByValue(day);

                                        if (dayIdx.HasValue)
                                        {
                                            ContProfileData profileData = ContProfileData.DeserializeProfileData(profilePath);
                                            profileData.GameDay = day;
                                            profileData.GameDayProgress = string.Empty;
                                            profileData.DayIdx = dayIdx.Value;
                                            profileData.NextDay = GetDayValueByIndex(dayIdx.Value + 1);

                                            profileData.SerializeProfileData(profilePath);

                                            return $"<xml><success>true</success><result><Success>true</Success><GameDay>{profileData.GameDay}</GameDay><GameDayProgress>{profileData.GameDayProgress}</GameDayProgress><NextDay>{profileData.NextDay}</NextDay>" +
                                                $"<Idx>{profileData.DayIdx}</Idx><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, $"{profileData.GameDay}{profileData.NextDay}", CurrentDate)}</confirm></result></xml>";
                                        }
                                        else
                                        {
                                            errMsg = $"[Xi2] - Cont: Requested day is invalid!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><success>false</success><error>Invalid day</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                        }
                                    }

                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "debug_get_days":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ContProfileData profileData = ContProfileData.DeserializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Day>{FormatDayDictionary()}</Day></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - Cont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - Cont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessCont</function></xml>";
                                }
                            case "ping":
                                return "<xml><success>true</success></xml>";
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
        }
    }

    public class ContProfileData
    {
        [XmlElement(ElementName = "GameDay")]
        public string GameDay { get; set; }

        [XmlElement(ElementName = "GameDayProgress")]
        public string GameDayProgress { get; set; }

        [XmlElement(ElementName = "NextDay")]
        public string NextDay { get; set; }

        [XmlElement(ElementName = "DayIdx")]
        public int DayIdx { get; set; }

        [XmlElement(ElementName = "UserType")]
        public int UserType { get; set; }

        [XmlElement(ElementName = "Stats")]
        public string Stats { get; set; }

        [XmlElement(ElementName = "Hash")]
        public string Hash { get; set; }

        public void SerializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContProfileData));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static ContProfileData DeserializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContProfileData));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (ContProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
