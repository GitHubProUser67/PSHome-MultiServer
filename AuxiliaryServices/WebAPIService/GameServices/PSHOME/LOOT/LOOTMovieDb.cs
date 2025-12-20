using CustomLogger;
using System.IO;

namespace WebAPIService.GameServices.PSHOME.LOOT
{
    public class LOOTMovieDb
    {

        public static string FetchDBInfo(string workPath, string id)
        {
            string LOOTTeleporterPath = $"{workPath}/LOOT/Teleporter";
            string LOOTDBPath = $"{workPath}/LOOT/MovieDB";

            switch (id)
            {
                case "60575C5C-98C649E2-A64DDF82-BC3002B5":
                    Directory.CreateDirectory(LOOTDBPath);
                    string movieDbJSONFilePath = $"{LOOTDBPath}/EOD.json";

                    if (File.Exists(movieDbJSONFilePath))
                    {
                        LoggerAccessor.LogInfo($"[LOOT] MovieDb - Found id:{id} JSON!");
                        return $"<parameter>{File.ReadAllText(movieDbJSONFilePath)}</parameter>";
                    }
                    else
                    {
                        LoggerAccessor.LogWarn($"[LOOT] MovieDb - No override id:{id} JSON found, using default!\nExpected path {movieDbJSONFilePath}");
                        bool logTicket = false;
#if DEBUG
                        logTicket = true;
#endif
                        return @$"<parameter>{{""reportUrl"":""https://alpha.lootgear.com/EOD/report.php"",""movieUrl"":""https://alpha.lootgear.com/EOD/movie.php"",""menuUrl"":""https://alpha.lootgear.com/EOD/menu.php""
                            ,""geoCodeUrl"":""https://alpha.lootgear.com/EOD/geoCode.php"",""pingDelay"":60,""npTicketEnabled"":true,""npTicketLogEnabled"":{logTicket.ToString().ToLower()}}}</parameter>";
                    }
                default:
                    Directory.CreateDirectory(LOOTTeleporterPath);
                    string teleporterJSONFilePath = $"{LOOTTeleporterPath}/Teleporter.json";

                    if (File.Exists(teleporterJSONFilePath))
                    {
                        LoggerAccessor.LogInfo($"[LOOT] Teleporter - Found Teleporter JSON!");
                        return $"<parameter>{File.ReadAllText(teleporterJSONFilePath)}</parameter>";
                    }
                    else
                    {
                        LoggerAccessor.LogWarn($"[LOOT] Teleporter - No override Teleporter JSON found, using default!\nExpected path {teleporterJSONFilePath}");
                        //NOT 100% yet working
                        return $"<parameter>{{\"g_destinations\":[{{\"sceneName\":\"tardis_open_house_b48d_2762\",\"name\":\"Destination 1\"}},{{\"sceneName\":\"pub_hollywood_hills_2d44_46fa\",\"name\":\"Destination 2\"}},{{\"sceneName\":\"stageset2_promo_c149_bd6e\",\"name\":\"Destination 3\"}}]}}</parameter>";
                    }
            }
        }
    }
}