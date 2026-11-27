using CustomLogger;
using System;

namespace WebAPIService.GameServices.PSHOME.OSHABERI
{
    public class OSHABERIClass
    {
        private readonly string workpath;
        private readonly string absolutepath;
        private readonly string fulluripath;
        private readonly string method;

        public OSHABERIClass(string method, string absolutepath, string workpath, string fulluripath)
        {
            this.method = method;
            this.absolutepath = absolutepath;
            this.workpath = workpath;
            this.fulluripath = fulluripath;
        }

        public string ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            PostData ??= Array.Empty<byte>();

            switch (method)
            {
                case "GET":
                    switch (absolutepath)
                    {
                        case "/game/app/announce/load_entrygate.php":
                            return Announce.loadEntrygate(workpath, fulluripath);

                        case "/game/app/announce/load_farm.php":
                            return Announce.loadFarm(workpath, fulluripath);

                        case "/game/app/announce/load_garden.php":
                            return Announce.loadGarden(workpath, fulluripath);

                        case "/game/app/userdata/load_userdata.php":
                            return UserData.loadUserData(PostData, ContentType, workpath, fulluripath, method);

                        case "/game/app/option/load_option.php":
                            return GlobalSetting.loadOption(workpath);

                        case "/game/app/campaign/load_campaign.php":
                            return Campaign.loadCampaign(workpath, fulluripath);

                        case "/game/app/event/load_user_event.php":
                            return EventRecords.loadUserEvent(PostData, ContentType, workpath, fulluripath, method);

                        default:
                            LoggerAccessor.LogWarn($"[OSHABERI] - Unknown GET request: {absolutepath}");
                            break;
                    }
                    break;

                case "POST":
                    switch (absolutepath)
                    {
                        case "/game/app/userdata/save_userdata.php":
                            return UserData.saveUserData(PostData, ContentType, workpath);

                        case "/game/app/event/save_user_event.php":
                            return EventRecords.saveUserEvent(PostData, ContentType, workpath);

                        default:
                            LoggerAccessor.LogWarn($"[OSHABERI] - Unknown POST request: {absolutepath}");
                            break;
                    }
                    break;

                default:
                    LoggerAccessor.LogWarn($"[OSHABERI] - Unsupported HTTP method: {method}");
                    break;
            }

            return null;
        }
    }
}