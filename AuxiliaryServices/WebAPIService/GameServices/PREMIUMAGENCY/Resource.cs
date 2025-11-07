using System.IO;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
#if !NETFRAMEWORK
using System.Web;
#endif
namespace WebAPIService.GameServices.PREMIUMAGENCY
{
    public class Resource
    {
        public static string getResourcePOST(byte[] PostData, string ContentType, string workpath, string fulluripath, string method)
        {
            string resKey = string.Empty;
            string resSeqNum = string.Empty;

            string regcd = string.Empty;

            if (method == "GET")
            {
#if NETFRAMEWORK
                resKey = HTTPProcessor.GetQueryParameters(fulluripath)["key"];
                resSeqNum = HTTPProcessor.GetQueryParameters(fulluripath)["seq"];
#else
                resKey = HttpUtility.ParseQueryString(fulluripath).Get("key");
                resSeqNum = HttpUtility.ParseQueryString(fulluripath).Get("seq");
#endif
            }
            else
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    resKey = data.GetParameterValue("key");
                    resSeqNum = data.GetParameterValue("seq");
                    regcd = data.GetParameterValue("regcd");

                    ms.Flush();
                }
            }

            LoggerAccessor.LogInfo($"Attempting to locate resource with Sequence:Key | {resSeqNum}:{resKey} ");

            #region EventController Paths
            string avatarfx = $"{workpath}/eventController/hs/avatarfx/Resources";
            string basaraCollabEventPath = $"{workpath}/eventController/collabo_iln/Resources";

            string DemoAsset_Settings130222 = $"{workpath}/eventController/hs/DemoAssets/130222/Resources";
            string StoreFrontGenerator_121212 = $"{workpath}/eventController/hs/StoreFrontGenerator/121212/Resources";
            string listenerfx = $"{workpath}/eventController/hs/listnerfx/Resources";
            string GundamPath = $"{workpath}/eventController/Gundam/Resources";
            string SCEAsiaHSMannedEvent2013 = $"{workpath}/eventController/hs/SCEAsia/MannedEvent/Resources";

            string fifaSimPredictorPath = $"{workpath}/eventController/Fifa/SimPred/Resources";
            string homecafeEnquetePath = $"{workpath}/eventController/hc/hc_Enquete/Resources";
            string homecafeGalleryPath = $"{workpath}/eventController/hc/hc_gallery/Resources";
            string homecafeShopPath = $"{workpath}/eventController/hc/hc_Shop/Resources";
            string homecafePlanPath = $"{workpath}/eventController/hc/hc_plan/Resources";

            string hs_commu_ev_cfg_130222 = $"{workpath}/eventController/hs/CommEvent/130222/Resources";
            string hs_userinfo_cfg = $"{workpath}/eventController/hs/UserInfo/Resources";


            string HSAquariumStatuePath = $"{workpath}/eventController/AquariumStatue";

            string Halloween2010Path = $"{workpath}/eventController/Halloween/2010/Resources";
            string HalloweenV22010Path = $"{workpath}/eventController/HalloweenV2/2010/Resources";
            string Halloween2010EffectsTriggerPath = $"{workpath}/eventController/HalloweenEffects/2010/Resources";
            string idolMasterLiveEventPath = $"{workpath}/eventController/iDOLMASTERs/LiveEvent/Resources";

            string idolMasterEventShopPath = $"{workpath}/eventController/iDOLMASTERs/EventShop/Resources";

            string j_liargame2Path = $"{workpath}/eventController/j_liargame2/Resources";
            string j_liargame2demoPath = $"{workpath}/eventController/j_liargame2demo/Resources";

            string jan2009infoboard = $"{workpath}/eventController/infoboard/01_09/Resources";
            string feb2009infoboard = $"{workpath}/eventController/infoboard/02_09/Resources";
            string july2009infoboard = $"{workpath}/eventController/infoboard/07_09/Resources";
			
            string JapanChristmas2010 = $"{workpath}/eventController/Christmas/2010/Resources";
            string MikuLiveEvent = $"{workpath}/eventController/MikuLiveEvent/Resources";
            string MikuLiveJukeboxPath = $"{workpath}/eventController/MikuLiveJukebox/Resources/Resources";
            string RollyJukeboxPath = $"{workpath}/eventController/RollyJukebox/Resources";
            string SonyAquariumPath = $"{workpath}/eventController/SonyAquarium/Resources";
            string shoeikingdomPath = $"{workpath}/eventController/ShoeiKingdom/Resources";

            string Spring2009 = $"{workpath}/eventController/Spring/2009/Resources";
            string Spring2013 = $"{workpath}/eventController/Spring/2013/Resources";

            string WhiteDay2010 = $"{workpath}/eventController/WhiteDay/2010/Resources";
            string WhiteDay2013 = $"{workpath}/eventController/WhiteDay/2013/Resources";
            #endregion

            switch (resKey)
            {
                #region SonyAquarium
                case "SonyAquarium_Blog":
                    {
                        Directory.CreateDirectory(SonyAquariumPath);
                        string filePath = $"{SonyAquariumPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                case "SonyAquarium_Config":
                    {
                        Directory.CreateDirectory(SonyAquariumPath);
                        string filePath = $"{SonyAquariumPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "SonyAquarium_VideoConfig":
                    {
                        Directory.CreateDirectory(SonyAquariumPath);
                        string filePath = $"{SonyAquariumPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                case "SonyAquarium_VideoScreens":
                    {
                        Directory.CreateDirectory(SonyAquariumPath);
                        string filePath = $"{SonyAquariumPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                case "SonyAquarium_Relocate":
                    {
                        Directory.CreateDirectory(HSAquariumStatuePath);
                        string filePath = $"{HSAquariumStatuePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region avatarfx
                case "avatarfx":
                    {
                        Directory.CreateDirectory(avatarfx);
                        string filePath = $"{avatarfx}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region CommunicationEvents
                case "hs_commu_ev_cfg_130222":
                    {
                        Directory.CreateDirectory(hs_commu_ev_cfg_130222);
                        string filePath = $"{hs_commu_ev_cfg_130222}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region hs_userinfo_cfg
                case "hs_userinfo_cfg":
                    {
                        Directory.CreateDirectory(hs_userinfo_cfg);
                        string filePath = $"{hs_userinfo_cfg}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region listenerfx
                case "listenerfx":
                    {
                        Directory.CreateDirectory(listenerfx);
                        string filePath = $"{listenerfx}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region DemoAssets
                case "DemoAsset_Settings130222":
                    {
                        Directory.CreateDirectory(DemoAsset_Settings130222);
                        string filePath = $"{DemoAsset_Settings130222}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region StoreFrontGenerator_121212
                case "StoreFrontGenerator_121212":
                    {
                        Directory.CreateDirectory(StoreFrontGenerator_121212);
                        string filePath = $"{StoreFrontGenerator_121212}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region j_LiarGame2
                case "j_liargame2_screen":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                case "j_liargame2_event":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "j_liargame2_schedule":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "j_liargame2_reward":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                case "j_liargame2_data":
                    {
                        Directory.CreateDirectory(j_liargame2Path);
                        string filePath = $"{j_liargame2Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region Liargame2 Demo ver
                case "j_liargame2demo_screen":
                    {
                        Directory.CreateDirectory(j_liargame2demoPath);
                        string filePath = $"{j_liargame2demoPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region Basara
                case "collabo_iln_def_scr":
                    {
                        Directory.CreateDirectory(basaraCollabEventPath);
                        string filePath = $"{basaraCollabEventPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                case "collabo_iln":
                    {
                        Directory.CreateDirectory(basaraCollabEventPath);
                        string filePath = $"{basaraCollabEventPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                #endregion

                #region theater_ev
                case "theater_ev_setting":
                    {
                        Directory.CreateDirectory(shoeikingdomPath);
                        string filePath = $"{shoeikingdomPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region Gundam
                case "jhome_square_gundam_time":
                    {
                        Directory.CreateDirectory(GundamPath);
                        string filePath = $"{GundamPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region InfoBoards

                case "jan2009":
                    {
                        Directory.CreateDirectory(feb2009infoboard);
                        string filePath = $"{feb2009infoboard}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<resource_data>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</resource_data>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}! \n Using fallback!");

                            string fallbackResXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<resource_data>
<result type=""int"">1</result>
<description type=""text"">RESOURCE_DATA</description>

<key type=""text"">jan2009</key>
<resourcecs>

<resource>
<seq type=""int"">1</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_1_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">2</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_2_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">3</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_3a_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">4</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_3b_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">5</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_4a_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">6</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_4b_mar1_090305.xml</data>
</resource>

</resourcecs>


</resource_data>";

                            return fallbackResXML;

                        }
                    }

                case "feb2009":
                    {
                        Directory.CreateDirectory(feb2009infoboard);
                        string filePath = $"{feb2009infoboard}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<resource_data>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</resource_data>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}! \n Using fallback!");

                            string fallbackResXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<resource_data>
<result type=""int"">1</result>
<description type=""text"">RESOURCE_DATA</description>

<key type=""text"">feb2009</key>
<resourcecs>

<resource>
<seq type=""int"">1</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_1_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">2</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_2_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">3</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_3a_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">4</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_3b_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">5</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_4a_mar1_090305.xml</data>
</resource>

<resource>
<seq type=""int"">6</seq>
<data type=""text"">http://sceasia-home.playstation.net/asia.home/events/for090305_asia/sg/ev_sq_rss_4b_mar1_090305.xml</data>
</resource>

</resourcecs>


</resource_data>";

                            return fallbackResXML;

                        }
                    }

                case "jul2009":
                    {
                        Directory.CreateDirectory(july2009infoboard);
                        string filePath = $"{july2009infoboard}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                #endregion

                #region SCEAsia HS Manned Event 2013

                case "SCEAsia_HS_ManEvent_2013 ":
                    {
                        Directory.CreateDirectory(SCEAsiaHSMannedEvent2013);
                        string filePath = $"{SCEAsiaHSMannedEvent2013}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;

                #endregion

                #region Spring Events

                case "spring2009":
                    {
                        Directory.CreateDirectory(Spring2009);
                        string filePath = $"{Spring2009}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;


                case "spring2013":
                    {
                        Directory.CreateDirectory(Spring2013);
                        string filePath = $"{Spring2013}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region WhiteDay2010
                case "whiteday_ev_2010":
                    {
                        Directory.CreateDirectory(WhiteDay2010);
                        string filePath = $"{WhiteDay2010}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion

                #region Halloween 2010

                case "halloween_ev_tot":
                    {
                        Directory.CreateDirectory(Halloween2010Path);
                        string filePath = $"{Halloween2010Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;

                case "halloween_demo_cfg":
                    {
                        Directory.CreateDirectory(HalloweenV22010Path);
                        string filePath = $"{HalloweenV22010Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;


                case "halloween_demo_cfg_dev":
                    {
                        Directory.CreateDirectory(HalloweenV22010Path);
                        string filePath = $"{HalloweenV22010Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;

                case "halloween_demo_totv2":
                    {
                        Directory.CreateDirectory(HalloweenV22010Path);
                        string filePath = $"{HalloweenV22010Path}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;

                case "halloween2010_event":
                    {
                        Directory.CreateDirectory(Halloween2010EffectsTriggerPath);
                        string filePath = $"{Halloween2010EffectsTriggerPath}/{resKey}.xml";

                        string fileEventPath = $"{Halloween2010EffectsTriggerPath}/dbg_seq_id.txt";

                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string EventFlagType;
                            if (File.Exists(fileEventPath))
                            {
                                EventFlagType = File.ReadAllText(fileEventPath);
                            }
                            else
                            {
                                LoggerAccessor.LogWarn($"Using fallback dbg_seq_id, missing entry at {fileEventPath}");
                                EventFlagType = "1";
                            }

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                "<resourcecs>\r\n\r\n" +
                                "<resource>\r\n" +
                                "<seq type=\"int\">1</seq>\r\n" +
                                $"<data type=\"text\">event_id=\"297\"event_xml_reskey=\"halloween2010_xml\"dbg_print=\"1\"event_xml_mem=\"1000\"localise_xml=\"http://playstationhome.jp/j.home/event/for101805/Halloween2010_localise{regcd}.xml\"dbg_seq_id=\"{EventFlagType}\"dbg_seq_time=\"30\"cloth_check_seqence=\"2\"cloth_check_interval=\"1.0\"effect_check_interval=\"5.0\"effect_interval=\"1.0\"eff_memory_limit=\"1000\"effect_instance_max=\"64\"effect_create_xzradius=\"10\"light_max=\"64\"infodisp_start_time=\"20\"infodisp_time_range=\"30\"</data>\r\n" +
                                $"</resource>\r\n\r\n" +
                                $"</resourcecs>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;


                case "halloween2010_xml":
                    {
                        Directory.CreateDirectory(Halloween2010EffectsTriggerPath);
                        string filePath = $"{Halloween2010EffectsTriggerPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;

                case "halloween2010tot_xml":
                    {
                        Directory.CreateDirectory(Halloween2010EffectsTriggerPath);
                        string filePath = $"{Halloween2010EffectsTriggerPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">RESOURCE_DATA</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;

                #endregion

                #region MikuLiveEvent
                case "concert_complete_judge":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_play_image":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_play_info":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_play_sound":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_wait_image":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_wait_info":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_wait_sound":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "concert_diva_image":
                    {
                        Directory.CreateDirectory(MikuLiveEvent);
                        string filePath = $"{MikuLiveEvent}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region HomeCafe Questionaire
                case "hc_Enquete_enable":
                    {
                        Directory.CreateDirectory(homecafeEnquetePath);
                        string filePath = $"{homecafeEnquetePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_Enquete_id":
                    {
                        Directory.CreateDirectory(homecafeEnquetePath);
                        string filePath = $"{homecafeEnquetePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_Enquete_reward":
                    {
                        Directory.CreateDirectory(homecafeEnquetePath);
                        string filePath = $"{homecafeEnquetePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_Enquete":
                    {
                        Directory.CreateDirectory(homecafeEnquetePath);
                        string filePath = $"{homecafeEnquetePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_Enquete_screen":
                    {
                        Directory.CreateDirectory(homecafeEnquetePath);
                        string filePath = $"{homecafeEnquetePath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region hc_gallery
                case "hc_gallery_main":
                    {
                        Directory.CreateDirectory(homecafeGalleryPath);
                        string filePath = $"{homecafeGalleryPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_gallery_skysetting":
                    {
                        Directory.CreateDirectory(homecafeGalleryPath);
                        string filePath = $"{homecafeGalleryPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region Homecafe Event Item Shop
                case "hc_shop":
                    {
                        Directory.CreateDirectory(homecafeShopPath);
                        string filePath = $"{homecafeShopPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region RollyCafe1F Plan
                case "hc_plan":
                    {
                        Directory.CreateDirectory(homecafePlanPath);
                        string filePath = $"{homecafePlanPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "hc_plan_error":
                    {
                        Directory.CreateDirectory(homecafePlanPath);
                        string filePath = $"{homecafePlanPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region MikuMusicSurvey
                case "miku_jukebox_mp4":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "miku_jukebox1_mp4":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "miku_jukebox1_board":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "miku_jukebox1_info":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "miku_jukebox1_time":
                    {
                        Directory.CreateDirectory(MikuLiveJukeboxPath);
                        string filePath = $"{MikuLiveJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region IdolMasters
                case "liveevent_idolmaster":
                    {
                        Directory.CreateDirectory(idolMasterLiveEventPath);
                        string filePath = $"{idolMasterLiveEventPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "shop_liveevent":
                    {
                        Directory.CreateDirectory(idolMasterEventShopPath);
                        string filePath = $"{idolMasterEventShopPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion

                #region Rolly Jukebox
                case "rolly_jukebox1_mp4":
                    {
                        Directory.CreateDirectory(RollyJukeboxPath);
                        string filePath = $"{RollyJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "rolly_jukebox1_info":
                    {
                        Directory.CreateDirectory(RollyJukeboxPath);
                        string filePath = $"{RollyJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "rolly_jukebox1_board":
                    {
                        Directory.CreateDirectory(RollyJukeboxPath);
                        string filePath = $"{RollyJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                case "rolly_jukebox1_time":
                    {
                        Directory.CreateDirectory(RollyJukeboxPath);
                        string filePath = $"{RollyJukeboxPath}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }
                    }
                    break;
                #endregion
				
                #region Japan Christmas 2010
                case "adventureReward":
                    {
                        Directory.CreateDirectory(JapanChristmas2010);
                        string filePath = $"{JapanChristmas2010}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                        
                case "adventureTool":
                    {
                        Directory.CreateDirectory(JapanChristmas2010);
                        string filePath = $"{JapanChristmas2010}/{resKey}.xml";
                        if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}!");
                        }

                    }
                    break;
                #endregion


                #region Fifa Simulator Predictor

                case "fifa_toto_data":
                    {
                        Directory.CreateDirectory(fifaSimPredictorPath);
                        string filePath = $"{fifaSimPredictorPath}/{resKey}.xml";
						if (File.Exists(filePath))
                        {
                            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - Resource with resource key {resKey} found and sent!");
                            string res = File.ReadAllText(filePath);

                            string resourceXML = "<xml>\r\n" +
                                "<result type=\"int\">1</result>\r\n" +
                                "<description type=\"text\">Success</description>\r\n" +
                                "<error_no type=\"int\">0</error_no>\r\n" +
                                "<error_message type=\"text\">None</error_message>\r\n" +
                                $"<key type=\"text\">{resKey}</key>\r\n" +
                                $"{res}\r\n" +
                                "</xml>";

                            return resourceXML;
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - Failed to find resource {resKey} with expected path {filePath}! Using a supported fallback!");

                            string fallbackXML = $@"<xml>
<result type=""int"">1</result>
<description type=""text"">RESOURCE_DATA</description>
<error_no type=""int"">0</error_no>
<error_message type=""text"">None</error_message>
<key type=""text"">fifa_toto_data</key>
<resource>
<seq type=""int"">1</seq>
<data type=""text"">event_id=""225""xp_ev_id=""226""event_data=""http://playstationhome.jp/eventController/Fifa/SimPredicator/Toto_Event.xml""result_data=""http://playstationhome.jp/eventController/Fifa/SimPredicator/Toto_ItemListData.xml""localise_data=""http://playstationhome.jp/eventController/Fifa/SimPredicator/Toto_Localise_{regcd}.xml""</data>
</resource>
</xml>";
                        }
                    }
                    break;

                #endregion

                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - Resource key {resKey} is unrecognized!\nPlease contact the developers!");

                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<key type=\"text\"></key>\r\n" +
                            "<resource>\r\n" +
                            "<seq type=\"int\">1</seq>\r\n" +
                            "<data type=\"text\"></data>\r\n" +
                            "</resource>\r\n" +
                            "</xml>";
                    }
            }

            return null;
        }

        public static string getInformationBoardSchedulePOST(byte[] PostData, string ContentType, string workpath, string eventId)
        {
            string resKey = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    resKey = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("key");

                    ms.Flush();
                }

                switch (eventId)
                {
                    default:
                        {
                            LoggerAccessor.LogError($"[PREMIUMAGENCY] - InfoBoardSchedule is unhandled with eventId {eventId} | ResKey {resKey}!");
                            return "<xml></xml>";
                        }
                }
            }

            return null;
        }
    }
}
