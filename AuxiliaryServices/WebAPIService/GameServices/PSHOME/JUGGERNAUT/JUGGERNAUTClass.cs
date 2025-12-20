using System;
using System.Collections.Generic;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.crafting;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT
{
    public class JUGGERNAUTClass
    {
        private string absolutepath;
        private string method;

        public JUGGERNAUTClass(string method, string absolutepath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
        }

        public string ProcessRequest(IDictionary<string, string> QueryParameters, string apiStaticPath, byte[] PostData = null, string ContentType = null)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "GET":
                    switch (absolutepath)
                    {
                        case "/clearasil/pushtelemetry.php":
                            if (QueryParameters != null && QueryParameters.ContainsKey("user") && QueryParameters.ContainsKey("timeingame"))
                            {
                                string user = QueryParameters["user"];
                                string timeingame = QueryParameters["timeingame"];
                                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(timeingame))
                                {
                                    try
                                    {
                                        int time = int.Parse(timeingame);
                                        CustomLogger.LoggerAccessor.LogInfo($"[JUGGERNAUT] - User: {user} spent {time / 60}:{time % 60} minutes in clearasil.");
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                return string.Empty;
                            }
                            break;
                        case "/clearasil/joinedspace.php":
                            return clearasil.joinedspace.ProcessJoinedSpace(QueryParameters, apiStaticPath);
                        case "/clearasil/getscores.php":
                            return clearasil.getscores.ProcessGetScores(QueryParameters, apiStaticPath);
                        case "/clearasil/pushrewards.php":
                            return clearasil.pushrewards.ProcessPushRewards(QueryParameters, apiStaticPath);
                        case "/clearasil/pushtime.php":
                            return clearasil.pushtime.ProcessPushTime(QueryParameters, apiStaticPath);
                        case "/clearasil/pushscore.php":
                            return clearasil.pushscore.ProcessPushScore(QueryParameters, apiStaticPath);
                        case "/farm/crafting_getxpstats.php":
                            return crafting_getxpstats.ProcessGetXpStats();
                        case "/farm/crafting_stats.php":
                            return crafting_stats.ProcessGetStats(apiStaticPath);
                        case "/farm/furniture_crafting_up.php":
                            return furniture_crafting_up.ProcessCraftingUp(QueryParameters, apiStaticPath);
                        case "/farm/furniture_slot_saved.php":
                            return furniture_slot_saved.ProcessSlotSaved(QueryParameters, apiStaticPath);
                        case "/farm/furniture_crafting_crafted.php":
                            return furniture_crafting_crafted.ProcessCraftingCrafted(QueryParameters, apiStaticPath);
                        case "/farm/furniture_crafting_down.php":
                            return furniture_crafting_down.ProcessCraftingDown(QueryParameters, apiStaticPath);
                        case "/farm/furniture_crafting_sold.php":
                            return furniture_crafting_sold.ProcessCraftingSold(QueryParameters, apiStaticPath);
                        case "/farm/resources_getall.php":
                            return farm.resources_getall.ProcessGetAll(QueryParameters, apiStaticPath);
                        case "/farm/weather_up.php":
                            return farm.weather_up.ProcessWeatherUp(QueryParameters, apiStaticPath);
                        case "/farm/wood_earned.php":
                            return farm.wood_earned.ProcessWoodEarned(QueryParameters, apiStaticPath);
                        case "/farm/remodel_getall.php":
                            return farm.remodel_getall.ProcessGetAll(QueryParameters, apiStaticPath);
                        case "/farm/remodel_bought.php":
                            return farm.remodel_bought.ProcessBought(QueryParameters, apiStaticPath);
                        case "/farm/animal_getall.php":
                            return animal_getall.ProcessGetAll(QueryParameters, apiStaticPath);
                        case "/farm/plant_getall.php":
                            return plant_getall.ProcessGetAll(QueryParameters, apiStaticPath);
                        case "/farm/plant_getxpstats.php":
                            return plant_getxpstats.ProcessGetXpStats();
                        case "/farm/plant_pushxp.php":
                            return plant_pushxp.ProcessPushXp(QueryParameters, apiStaticPath);
                        case "/farm/plant_bought.php":
                            return plant_bought.ProcessBought(QueryParameters, apiStaticPath);
                        case "/farm/plant_leveled.php":
                            return plant_leveled.ProcessLeveled(QueryParameters, apiStaticPath);
                        case "/farm/plant_watered.php":
                            return plant_watered.ProcessWatered(QueryParameters, apiStaticPath);
                        case "/farm/plant_getxp.php":
                            return plant_getxp.ProcessGetXp(QueryParameters, apiStaticPath);
                        case "/farm/plant_sold.php":
                            return plant_sold.ProcessSold(QueryParameters, apiStaticPath);
                        case "/farm/plant_stats.php":
                            return plant_stats.ProcessStats(apiStaticPath);
                        case "/farm/animal_bought.php":
                            return animal_bought.ProcessBought(QueryParameters, apiStaticPath);
                        case "/farm/animal_sold.php":
                            return animal_sold.ProcessSold(QueryParameters, apiStaticPath);
                        case "/farm/animal_leveled.php":
                            return animal_leveled.ProcessLeveled(QueryParameters, apiStaticPath);
                        case "/farm/animal_fed.php":
                            return animal_fed.ProcessFed(QueryParameters, apiStaticPath);
                        case "/farm/animal_renewed.php":
                            return animal_renewed.ProcessRenewed(QueryParameters, apiStaticPath);
                        case "/farm/animal_collect_renew.php":
                            return animal_collect_renew.ProcessCollectRenew(QueryParameters, apiStaticPath);
                        case "/farm/animal_stats.php":
                            return animal_stats.ProcessStats(apiStaticPath);
                        case "/farm/furniture_down.php":
                            return furniture_down.ProcessDown(QueryParameters, apiStaticPath);
                        case "/cutteridge/effects2012chances.php":
                            return cutteridge.effects2012chances.ProcessChances(apiStaticPath);
                        default:
                            break;
                    }
                    break;
                case "POST":
                    switch (absolutepath)
                    {
                        case "/farm/furniture_up.php":
                            return furniture_up.ProcessUp(PostData, ContentType, apiStaticPath);
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
